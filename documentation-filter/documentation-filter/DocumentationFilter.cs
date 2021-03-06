﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Linq;


namespace TomasHalac.DocumentationFilter {

    /// <summary>
    /// This tool, allows to remove blocks of documentation generated by the C# comiler according to accessibility conditions.
    /// Git URL: https://github.com/tomashalac/documentation-filter
    /// </summary>
    public class DocumentationFilter {

        private readonly List<string> ListNamespace;
        private readonly Assembly AssemblyLoaded;
        private readonly string FullFileName, ExportXml;

        /// <summary>
        /// This object allows you to manipulate the specified documentation.
        /// </summary>
        /// <param name="fullFileName">The relative path to the file, example "docs.xml"</param>
        /// <param name="dllToLoad">The absolute path to the DLL, example "C:\test\MyCode.dll"</param>
        public DocumentationFilter(string fullFileName, string dllToLoad) {
            this.FullFileName = fullFileName;
            this.ExportXml = "new_" + fullFileName;

            AssemblyLoaded = Assembly.LoadFile(dllToLoad);

            ListNamespace = AssemblyLoaded.GetExportedTypes().Select(t => t.Namespace).Distinct().ToList();

            if (File.Exists(fullFileName) == false) {
                throw new FileNotFoundException("The .xml was not found", fullFileName);
            }
        }

        /// <summary>
        /// Move all summaries that are public to the other file "new_{fullFileName}.xml"
        /// </summary>
        public void MoveOnlyPublics() {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(FullFileName);

            var doc = xmlDocument.LastChild;

            foreach (XmlNode item in doc.SelectNodes("members/*")) {

                //M:<namespace>.<class>.#ctor(System.String)
                //M:<namespace>.<class>.#ctor
                //T:<namespace>.<class>
                var function = item.Attributes["name"].Value;

                var XMLType = function[0];

                var part1 = "";
                var args = "";
                string funtionName = "";
                string typeName = "";
                string inNamespace = "";

                if (function.Contains("(")) {
                    part1 = function.Substring(2, function.IndexOf("(") - 2);
                    args = function.Substring(function.IndexOf("("));
                } else {
                    part1 = function.Substring(2);
                }

                
                foreach(var @namespace in ListNamespace) {
                    if (part1.Contains(@namespace)) {
                        part1 = part1.Replace(@namespace + ".", "");
                        inNamespace = @namespace;
                    }
                }


                if (part1.Contains(".")) {
                    typeName = part1.Substring(0, part1.IndexOf("."));
                    funtionName = part1.Substring(typeName.Length + 1);
                } else {
                    typeName = part1;
                }

                //if it's a generic, I pass it to a format that works with reflection
                if (funtionName.Contains("`")) {
                    funtionName = funtionName.Substring(0, funtionName.IndexOf("`"));
                }

                //if it is an enum, it has another dot...
                if (funtionName.Contains(".")) {
                    //I handle it as a type because I only want to know if it's public
                    XMLType = 'T';
                }



                bool isPublic = false, typePublic = false;

                try {
                    Type type = GetType(inNamespace + "." + typeName, this.AssemblyLoaded);

                    typePublic = Type(type);

                    if (XMLType == 'T') {
                        isPublic = typePublic;
                    } else if (XMLType == 'F') {
                        isPublic = Field(type, funtionName);
                    } else if (XMLType == 'P') {
                        isPublic = Property(type, funtionName);
                    } else if (XMLType == 'M') {
                        if (funtionName == "#ctor") {
                            isPublic = MethodConstructor(type, args);
                        } else {
                            isPublic = Method(type, funtionName, args);
                        }
                    } else {
                        throw new Exception("The type '" + XMLType + "' is not recognized.");
                    }
                } catch (Exception e) {
                    Console.WriteLine("\n\n\nError in: " + function + "\n\n" + e.ToString() + "\n\n\n");
                }


                Console.WriteLine("Item: " + (isPublic && typePublic) + " => " + function + "  " + item.FirstChild.InnerText);
                Console.WriteLine("");

                if ((isPublic && typePublic) == false) {
                    item.ParentNode.RemoveChild(item);
                }

            }

            xmlDocument.Save(ExportXml);
        }



        /*  T	type: class, interface, struct, enum, delegate
            F	field
            P	property (including indexers or other indexed properties)
            M	method (including such special methods as constructors, operators, and so forth)
        */
        internal static bool Type(Type type) {
            return type.IsPublic;
        }

        internal static bool Field(Type type, string fieldName) {
            return type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static).IsPublic;
        }

        internal static bool Property(Type type, string propertyName) {
            var prop = type.GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            return prop.GetGetMethod().IsPublic || prop.GetSetMethod().IsPublic;
        }

        internal static bool Method(Type type, string methodName, string methodArgs) {
            string testsLog = "";

            foreach (MethodInfo method in type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)) {

                if (method.Name == methodName) {
                    var test = MethodToArgsString(method);
                    if (test == methodArgs) {
                        return method.IsPublic;
                    }
                    testsLog += test + "\n";
                }
            }

            throw new Exception("The Method was not found, tests: " + testsLog);
        }

        internal static bool MethodConstructor(Type type, string methodArgs) {
            //#ctor
            string testsLog = "";
            foreach (ConstructorInfo method in type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)) {

                var test = MethodToArgsString(method);
                if (test == methodArgs) {
                    return method.IsPublic;
                }
                testsLog += test + "\n";
            }

            throw new Exception("The Constructor was not found, tests: " + testsLog);
        }

        internal static Type GetType(string fullTypeName, Assembly assemblyLoaded) {
            var type = assemblyLoaded.GetType(fullTypeName);

            if (type == null)
                throw new Exception("The Type was not found, name: " + fullTypeName);

            return type;
        }

        internal static string MethodToArgsString(MethodBase method) {
            string parms = "";
            foreach (var item in method.GetParameters()) {

                string type = item.ParameterType.FullName.Replace("+", ".");
                parms += type + ",";
            }

            //to remove the last ', '
            if (parms.Length > 0) {
                parms = parms.Substring(0, parms.Length - 1);
            }


            parms = "(" + parms + ")";
            if (parms == "()")
                return "";

            return parms;
        }

    }
}
