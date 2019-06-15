using NUnit.Framework;
using System;
using System.IO;
using TomasHalac.DocumentationFilter;

namespace TomasHalac.DocumentationFilter.QualityAssurance {

    /// <summary>
    /// A public summary
    /// </summary>
    public class Tests {

        /// <summary>
        ///  A public summary
        /// </summary>
        public Tests() { }
        private Tests(string arg, NullReferenceException e) { }

        /// <summary>
        /// A public summary
        /// </summary>
        public float Property {
            get {
                return 1;
            }
        }

        /// <summary>
        /// A public summary
        /// </summary>
        public int Var1;

        /// <summary>
        /// A private summary
        /// </summary>
        private string Var2;

        /// <summary>
        /// A public summary
        /// </summary>
        public string[] MethodTest1(int a, Stream stream) { return new string[3]; }

        /// <summary>
        /// A public summary
        /// </summary>
        [SetUp]
        public void Run() {  }

        /// <summary>
        /// A public summary
        /// </summary>
        [Test]
        public void Integration_FilterThisProyectDocumentation() {
            File.Delete("new_documentation-filter-tests.xml");

            var docs = new DocumentationFilter("documentation-filter-tests.xml", Environment.CurrentDirectory + "/documentation-filter-tests.dll");
            Assert.False(File.Exists("new_documentation-filter-tests.xml"));
            docs.MoveOnlyPublics();

            Assert.True(File.Exists("new_documentation-filter-tests.xml"));

            string original = File.ReadAllText("documentation-filter-tests.xml");
            string newXml = File.ReadAllText("new_documentation-filter-tests.xml");

            Assert.False(newXml.Contains("private", System.StringComparison.InvariantCultureIgnoreCase));

            Assert.AreEqual(original.Split("public").Length, newXml.Split("public").Length);
        }

        /// <summary>
        ///  A public summary
        /// </summary>
        [Test]
        public void Unit_MethodToArgsString() {
            var method = typeof(Tests).GetMethod("MethodTest1");
            Assert.NotNull(method);

            var expected = "(System.Int32,System.IO.Stream)";
            var actual = DocumentationFilter.MethodToArgsString(method);

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///  A public summary
        /// </summary>
        [Test]
        public void Unit_GetType() {
            var actual = DocumentationFilter.GetType("TomasHalac.DocumentationFilter.QualityAssurance.Tests", typeof(Tests).Assembly);
            Assert.AreEqual(typeof(Tests), actual);
        }


        /// <summary>
        ///  A public summary
        /// </summary>
        [Test]
        public void Unit_GetType2() {
            try {
                DocumentationFilter.GetType("TomasHalac.DocumentationFilter.QualityAssurance.NotFound", typeof(Tests).Assembly);
                Assert.Fail();
            } catch (System.Exception) {

            }
        }


        /// <summary>
        ///  A public summary
        /// </summary>
        [Test]
        public void Unit_MethodConstructor() {
            var constructor = typeof(Tests).GetConstructors(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)[0];
            Assert.NotNull(constructor);

            var actual = DocumentationFilter.MethodConstructor(typeof(Tests), "(System.String,System.NullReferenceException)");
            Assert.AreEqual(constructor.IsPublic, actual);
        }


        /// <summary>
        ///  A public summary
        /// </summary>
        [Test]
        public void Unit_MethodConstructor2() {
            try {
                DocumentationFilter.MethodConstructor(typeof(Tests), "(System.Int32,System.IO.Stream)");
                Assert.Fail();
            } catch (System.Exception) {

            }
        }

        /// <summary>
        ///  A public summary
        /// </summary>
        [Test]
        public void Unit_Method() {
            var actual = DocumentationFilter.Method(typeof(Tests), "MethodTest1", "(System.Int32,System.IO.Stream)");
            Assert.AreEqual(true, actual);
        }


        /// <summary>
        ///  A public summary
        /// </summary>
        [Test]
        public void Unit_Method2() {
            try {
                DocumentationFilter.Method(typeof(Tests), "MethodTest2", "(System.Int32,System.IO.Stream)");
                Assert.Fail();
            } catch (System.Exception) {

            }
        }


        /// <summary>
        ///  A public summary
        /// </summary>
        [Test]
        public void Unit_Method3() {
            try {
                DocumentationFilter.Method(typeof(Tests), "MethodTest1", "(System.String,System.NullReferenceException)");
                Assert.Fail();
            } catch (System.Exception) {

            }
        }


        /// <summary>
        ///  A public summary
        /// </summary>
        [Test]
        public void Unit_Property() {
            var actual = DocumentationFilter.Property(typeof(Tests), "Property");
            Assert.AreEqual(true, actual);
        }

        /// <summary>
        ///  A public summary
        /// </summary>
        [Test]
        public void Unit_Field() {
            var actual = DocumentationFilter.Field(typeof(Tests), "Var1");
            Assert.AreEqual(true, actual);
        }


        /// <summary>
        ///  A public summary
        /// </summary>
        [Test]
        public void Unit_Field2() {
            var actual = DocumentationFilter.Field(typeof(Tests), "Var2");
            Assert.AreEqual(false, actual);
        }

        /// <summary>
        ///  A public summary
        /// </summary>
        [Test]
        public void Unit_Type() {
            var actual = DocumentationFilter.Type(typeof(Tests));
            Assert.AreEqual(true, actual);
        }

        /// <summary>
        ///  A public summary
        /// </summary>
        [Test]
        public void Unit_Type2() {
            var actual = DocumentationFilter.Type(typeof(Tests.PrivateClass));
            Assert.AreEqual(false, actual);
        }

        private class PrivateClass {

        }

    }

}