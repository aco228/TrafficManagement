using System;
using System.Collections.Generic;
using System.Linq;
using TrafficManagement.Wpf.Importer.Core.Csv.Read.Models;
using TrafficManagement.Wpf.Importer.Direct;

namespace TrafficManagement.Wpf.Importer.Csv.Write
{
    public static class CsvWriterCustom
    {

        public static void ClearCurrentConsoleLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }


        public static void WriteToDatabase(List<DynamicCsv> allCsvRecordsList, int maxRecordsAtOnce)
        {
            // separate (max 1000) records from allCsvs
            // search msisdn records from those
            // remove found ones
            // insert to db
            // remove those (max 1000) records from allCsvs
            // continue till all are inserted

            var remainingRecords = new List<DynamicCsv>();
            remainingRecords.AddRange(allCsvRecordsList);
            var xRecordsToCheck = new List<DynamicCsv>();

            var alreadyInsertedRecordsCount = 0;
            var newlyInsertedRecords = 0;

            while (remainingRecords.Count > 0)
            {

                //Console.Clear();

                if (remainingRecords.Count >= maxRecordsAtOnce)
                {
                    xRecordsToCheck.AddRange(remainingRecords.Take(maxRecordsAtOnce));
                    remainingRecords.RemoveRange(0, maxRecordsAtOnce);
                }
                else if (remainingRecords.Count < maxRecordsAtOnce)
                {
                    xRecordsToCheck.AddRange(remainingRecords);
                    remainingRecords.Clear();
                }


                string msisdnParams = "";
                xRecordsToCheck.ForEach(record => msisdnParams += "'" + record.Msisdn + "'" + ",");
                msisdnParams = msisdnParams.Remove(msisdnParams.Length - 1, 1);

                List<string> existingMsisdns = DirectReader.QueryMsisdnsFromTm_Lead(msisdnParams);
                var recordsToInsert = new List<string[]>();

                foreach (var record in xRecordsToCheck)
                {
                    if (!existingMsisdns.Contains(record.Msisdn))
                    {
                        recordsToInsert.Add(DynamicCsvHelper.DynamicCsvToParamsList(record));
                    }
                    else
                    {
                        alreadyInsertedRecordsCount++;
                        //Console.WriteLine("Already inserted records count - " + alreadyInsertedRecordsCount);
                        //Console.SetCursorPosition(0, Console.CursorTop - 1);
                        //ClearCurrentConsoleLine();
                    }
                }

                //Console.WriteLine("Already inserted records count - " + alreadyInsertedRecordsCount);

                // finally insert x number of checked records
                var insertionResult = DirectWriter.InsertCsvData(recordsToInsert);
                if (insertionResult.isSuccess)
                {
                    newlyInsertedRecords++;
                    //Console.WriteLine("New inserted records count - " + newlyInsertedRecords);
                    //Console.SetCursorPosition(0, Console.CursorTop - 1);
                    //ClearCurrentConsoleLine();
                }
                else
                {
                    //Console.WriteLine("Failed to insert record - " + recordsToInsert);
                }

                //Console.WriteLine("New inserted records count - " + newlyInsertedRecords);

            }


        }

    }
}
