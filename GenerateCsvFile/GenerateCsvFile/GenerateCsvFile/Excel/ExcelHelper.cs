using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using GenerateCsvFile.DataContract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GenerateCsvFile.Excel
{
    public class ExcelHelper
    {


        // Given a document name and text, 
        // inserts a new worksheet and writes the text to cell "A1" of the new worksheet.
        public MemoryStream CreateExcel(List<FileRecord> lstFileRecord)
        {
            MemoryStream streamData = new MemoryStream();
            using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(streamData, SpreadsheetDocumentType.Workbook))
            {
                // Add a WorkbookPart to the document.
                WorkbookPart workbookpart = spreadsheetDocument.AddWorkbookPart();
                workbookpart.Workbook = new Workbook();

                // Add a WorksheetPart to the WorkbookPart.
                WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
                SheetData sheetData = new SheetData();
                worksheetPart.Worksheet = new Worksheet(sheetData);

                // Add Sheets to the Workbook.
                Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.
                    AppendChild<Sheets>(new Sheets());

                // Append a new worksheet and associate it with the workbook.
                Sheet sheet = new Sheet()
                {
                    Id = spreadsheetDocument.WorkbookPart.
                    GetIdOfPart(worksheetPart),
                    SheetId = 1,
                    Name = "ImageResults"
                };

                UInt32Value i = 1;
                Row row = new Row() { RowIndex = i };
                Cell header1 = new Cell() { CellReference = "A1", CellValue = new CellValue("Image1"), DataType = CellValues.String };
                row.Append(header1);
                Cell header2 = new Cell() { CellReference = "B1", CellValue = new CellValue("Image2"), DataType = CellValues.String };
                row.Append(header2);
                Cell header3 = new Cell() { CellReference = "C1", CellValue = new CellValue("Score"), DataType = CellValues.String };
                row.Append(header3);
                Cell header4 = new Cell() { CellReference = "D1", CellValue = new CellValue("EllapsedTime(s)"), DataType = CellValues.String };
                row.Append(header4);
                sheetData.Append(row);

                foreach (var record in lstFileRecord)
                {
                    i++;
                    row = new Row() { RowIndex = i };
                    Cell cell1 = new Cell() { CellReference = "A" + i, CellValue = new CellValue(record.Image1), DataType = CellValues.String };
                    row.Append(cell1);
                    Cell cell2 = new Cell() { CellReference = "B" + i, CellValue = new CellValue(record.Image2), DataType = CellValues.String };
                    row.Append(cell2);
                    Cell cell3 = new Cell() { CellReference = "C" + i, CellValue = new CellValue(record.Score), DataType = CellValues.Number };
                    row.Append(cell3);
                    Cell cell4 = new Cell() { CellReference = "D" + i, CellValue = new CellValue(record.ElapsedTime), DataType = CellValues.String };
                    row.Append(cell4);
                    sheetData.Append(row);
                }

                sheets.Append(sheet);
                workbookpart.Workbook.Save();

                // Close the document.
                spreadsheetDocument.Close();
                return streamData;
            }
        }
    }
}
