using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("documentation-filter-tests")]
namespace TomasHalac.DocumentationFilter {
    class Program {
        static void Main() {
            Console.WriteLine("Exporting the public documentation of this project!, end file: '" + Environment.CurrentDirectory + @"\new_documentation-filter.xml'");

            var doc = new DocumentationFilter("documentation-filter.xml", Environment.CurrentDirectory + "/documentation-filter.dll");
            doc.MoveOnlyPublics();

        }
    }
}
