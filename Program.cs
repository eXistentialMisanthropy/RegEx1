using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

using CsvHelper;

using RegEx1.Models;

namespace RegEx1
{
    class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine(@"Please Enter Input File Path: ");
            string path = Console.ReadLine();

            var outputPath = Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path) + "_processed" + Path.GetExtension(path);

            using (var reader = new StreamReader(path))
            {
                using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var inputRecords = csvReader.GetRecords<Input>();
                    var lineNumber = 1;

                    using (var writer = new StreamWriter(outputPath))
                    {
                        using (var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
                        {
                            foreach (var record in inputRecords)
                            {
                                var dateRegex = new Regex(@"\d?\d\/\d?\d\/\d\d\d\d");
                                var dateMatch = dateRegex.Match(record.Comments);

                                var emailRegex = new Regex(@"\A[a-z0-9!#$%&'*+/=?^_‘{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_‘{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\z");
                                var emailMatch = emailRegex.Match(record.Comments);

                                var phoneNumberRegex = new Regex(@"\(?([0-9]{3})\)?([ .-]?)([0-9]{3})\2([0-9]{4})");
                                var phoneNumberMatch = phoneNumberRegex.Match(record.Comments);

                                var creditCardNumberRegex = new Regex(@"\d\d\d\d[- ]?\d\d\d\d[- ]?\d\d\d\d[- ]?\d\d\d\d");
                                var creditCardNumberMatch = creditCardNumberRegex.Match(record.Comments);

                                var output = new Output()
                                {
                                    Comments = record.Comments,
                                    DateString = dateMatch.Value,
                                    Email = emailMatch.Value,
                                    PhoneNumber = phoneNumberMatch.Value,
                                    CreditCardNumber = creditCardNumberMatch.Value
                                };

                                if (lineNumber == 1)
                                {
                                    csvWriter.WriteHeader<Output>();
                                    csvWriter.NextRecord();
                                }
                                else
                                {
                                    Console.WriteLine($"{DateTime.Now}: Processed Line {lineNumber}");
                                    csvWriter.WriteRecord(output);
                                    csvWriter.NextRecord();
                                }

                                lineNumber++;
                            }
                        }
                    }
                }
            }            

            Console.WriteLine($"DONE: {outputPath}");
            Console.WriteLine("Press any key to close");
            Console.ReadKey();
        }
    }
}
