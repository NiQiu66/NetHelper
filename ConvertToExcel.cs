using System;

/// <summary>
/// Summary description for Class1
/// </summary>
public class Class1
{
	public Class1()
	{
        var data =new List<Data>();

        var workbook = new XSSFWorkbook();
        var sheet = workbook.CreateSheet();

        var row = sheet.CreateRow(0);
        var cell = null as ICell;
        var cellNum = 0;

        #region title

        row.CreateCell(cellNum++).SetCellValue("ID");
        row.CreateCell(cellNum++).SetCellValue("名字");

        cellNum = 0;
        sheet.SetColumnWidth(cellNum++, 20 * 256);
        sheet.SetColumnWidth(cellNum++, 20 * 256);

        #endregion
        #region content
        if (data != null && data.Any())
        {
            var i = 0;
            foreach (var item in data)
            {
                cellNum = 0;
                row = sheet.CreateRow(i + 1);
                row.CreateCell(cellNum++).SetCellValue(item.ID);
                row.CreateCell(cellNum++).SetCellValue(item.Name);
                i++;
            }
        }
        #endregion

        var ms = new MemoryStream();
        workbook.Write(ms);
        return File(ms.ToArray(), "application/x-xls", $"文件名.xlsx");
    }

    public class Data
    {
        public int ID;
        public string Name;
    }
}
