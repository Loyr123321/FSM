using BlazorApp1.Models;
using BlazorApp1.Views;
using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;

namespace BlazorApp1.Utils
{
    public class ExcelUtils
    {
        public static string TT(string sheetName, List<string> headers, List<OrderGridView> values)
        {
            try
            {
                HSSFWorkbook hssfworkbook = new HSSFWorkbook();

                //
                DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
                dsi.Company = "";
                hssfworkbook.DocumentSummaryInformation = dsi;

                SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
                si.Subject = "";
                hssfworkbook.SummaryInformation = si;
                //

                //Вставить хотябы 1 лист в противном случае Excel сообщит «данные потеряны в файле
                var sheet = hssfworkbook.CreateSheet(sheetName);
                ((HSSFSheet)hssfworkbook.GetSheetAt(0)).AlternativeFormula = false;
                ((HSSFSheet)hssfworkbook.GetSheetAt(0)).AlternativeExpression = false;


                //Styles
                ICellStyle myHeaderStyle = hssfworkbook.CreateCellStyle();
                myHeaderStyle.ShrinkToFit = true;
                myHeaderStyle.WrapText = true;
                myHeaderStyle.Alignment = HorizontalAlignment.Center;
                myHeaderStyle.VerticalAlignment = VerticalAlignment.Center;
                //myHeaderStyle.BorderTop = BorderStyle.Medium;
                //myHeaderStyle.BorderBottom= BorderStyle.Medium;
                //myHeaderStyle.BorderLeft = BorderStyle.Medium;
                //myHeaderStyle.BorderRight = BorderStyle.Medium;
                //Шрифт заголовков
                var headerFont = hssfworkbook.CreateFont();
                headerFont.FontHeightInPoints = 12;
                headerFont.FontName = "Arial";
                headerFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
                myHeaderStyle.SetFont(headerFont);


                ICellStyle myStyle = hssfworkbook.CreateCellStyle();
                myStyle.ShrinkToFit = true;
                myStyle.WrapText = true;
                myStyle.Alignment = HorizontalAlignment.Center;
                myStyle.VerticalAlignment = VerticalAlignment.Center;
                //Шрифт
                var font = hssfworkbook.CreateFont();
                font.FontHeightInPoints = 8;
                font.FontName = "Arial";
                font.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.None;
                myHeaderStyle.SetFont(font);
                //------------------------------------------------------

                //Заголовки
                IRow row = sheet.CreateRow(0);
                for (int i = 0; i < headers.Count; i++)
                {
                    ICell cell = row.CreateCell(i);
                    cell.SetCellValue(headers[i]);
                    cell.CellStyle = myHeaderStyle;
                    cell.CellStyle.SetFont(headerFont);
                }

                //values == List<OrderGridView>() Заказы
                //if (typeof(BlazorApp1.Views.OrderGridView).Equals(typeof(T)))
                //{
                //}

                //Сами данные
                ICell mycell;
                int my_i=1;
                /*
                foreach (OrderGridView oneValue in values)
                {
                    row = sheet.CreateRow(my_i);

                    //#
                    int? v0 = oneValue.Order.OrderNum;
                    mycell = row.CreateCell(0);
                    mycell.SetCellValue(v0.ToString());
                    mycell.CellStyle = myStyle;

                    //№ Заявки ЛМ
                    string? v1 = oneValue.Fields[0].FieldValueAsString;
                    if (!string.IsNullOrEmpty(v1))
                    {
                        mycell = row.CreateCell(1);
                        mycell.SetCellValue(v1.ToString());
                        mycell.CellStyle = myStyle;
                    }
                    
                    //Контакты
                    var v2 = oneValue.Order.Contacts;
                    string v2_val= string.Empty;
                    if (v2 != null && v2.Count>0) 
                    {
                        foreach (ClientContact contact in v2)
                        {
                            v2_val += contact.ClientContactName +": "+ contact.Phone + Environment.NewLine;
                        }
                        mycell = row.CreateCell(2);
                        mycell.SetCellValue(v2_val.ToString());
                        mycell.CellStyle = myStyle;
                    }

                    //Address
                    var v3 = oneValue.Order.Address.FullAddress;
                    if(v3 != null)
                    {
                        mycell = row.CreateCell(3);
                        mycell.SetCellValue(v3.ToString());
                        mycell.CellStyle = myStyle;
                    }
                    
                    //Район/Станция
                    var v4 = oneValue.Order.Regions;
                    string v4_val = string.Empty;
                    if (v4 != null && v4.Count > 0)
                    {
                        foreach (RegionValue rv in v4)
                        {
                            v4_val += rv.Value + Environment.NewLine;
                        }
                        mycell = row.CreateCell(4);
                        mycell.SetCellValue(v4_val.ToString());
                        mycell.CellStyle = myStyle;
                    }

                    //Магазин
                    var v5 = oneValue.Fields[1].FieldValueAsString;
                    if (v5 != null)
                    {
                        mycell = row.CreateCell(5);
                        mycell.SetCellValue(v5.ToString());
                        mycell.CellStyle = myStyle;
                    }

                    //Кол-Во
                    var v6 = oneValue.Fields[2].FieldValueAsString;
                    if (v6 != null)
                    {
                        mycell = row.CreateCell(6);
                        //mycell.SetCellValue(v6.ToString());
                        mycell.SetCellValue(oneValue.Fields[2].FieldValueAsNumeric);
                        mycell.SetCellType(CellType.Numeric);
                        mycell.CellStyle = myStyle;
                    }


                    //Тип заказа
                    var v7 = oneValue.Order.Template.OrderType;
                    if (v7 != null)
                    {
                        mycell = row.CreateCell(7);
                        mycell.SetCellValue(v7.ToString());
                        mycell.CellStyle = myStyle;
                    }

                    //Стоимость
                    var v8 = oneValue.Fields[3].FieldValueAsString;
                    if (v8 != null)
                    {
                        mycell = row.CreateCell(8);
                        //mycell.SetCellValue(v8.ToString());
                        mycell.SetCellValue(oneValue.Fields[3].FieldValueAsNumeric);
                        mycell.SetCellType(CellType.Numeric);
                        mycell.CellStyle = myStyle;
                    }

                    //Дата создания заказа
                    var v9 = oneValue.Order.CreateTime;
                    if (v9 != null)
                    {
                        mycell = row.CreateCell(9);
                        mycell.SetCellValue(v9.ToString());
                        mycell.CellStyle = myStyle;
                    }

                    //Дата завершения заказа монтажником
                    var v10 = oneValue.Order.StopTime;
                    if (v10 != null)
                    {
                        mycell = row.CreateCell(10);
                        mycell.SetCellValue(v10.ToString());
                        mycell.CellStyle = myStyle;
                    }

                    //Статус
                    var v11 = oneValue.Order.OrderStatus.StatusName;
                    if (v11 != null)
                    {
                        mycell = row.CreateCell(11);
                        mycell.SetCellValue(v11.ToString());
                        mycell.CellStyle = myStyle;
                    }

                    //
                    int c1= 12;
                    for(int i=4; i< oneValue.Fields.Count; i++)
                    {
                        string val0 = oneValue.Fields[i].FieldValueAsString;
                        
                        if (val0 != null)
                        {
                            mycell = row.CreateCell(c1);

                            if (oneValue.Fields[i].IsNumeric)
                            {
                                mycell.SetCellValue(oneValue.Fields[i].FieldValueAsNumeric);
                                mycell.SetCellType(CellType.Numeric);
                            }
                            else
                            {
                                mycell.SetCellValue(val0);
                            }
                            
                            mycell.CellStyle = myStyle;
                        }
                        c1++;
                    }
                    //

                    my_i++;
                }
                */
                //Ширина столбцов
                sheet.SetColumnWidth(0, 15 * 256);
                sheet.SetColumnWidth(1, 15 * 256);
                sheet.SetColumnWidth(2, 60 * 256);//Контакты
                sheet.SetColumnWidth(3, 40 * 256);//Адрес
                sheet.SetColumnWidth(4, 30 * 256);//РайонСтанция
                sheet.SetColumnWidth(5, 20 * 256);//Магазин
                sheet.SetColumnWidth(6, 15 * 256);//Кол-во
                sheet.SetColumnWidth(7, 20 * 256);//Тип услуг
                sheet.SetColumnWidth(8, 15 * 256);//Стоимость
                sheet.SetColumnWidth(9, 30 * 256);//Дата создания заказа
                sheet.SetColumnWidth(10, 30 * 256);//Дата завершения заказа
                sheet.SetColumnWidth(11, 15 * 256);//Статус
                sheet.SetColumnWidth(12, 30 * 256);//Компенсация Леруа
                sheet.SetColumnWidth(13, 40 * 256);//Акты невыполненных работ
                sheet.SetColumnWidth(14, 30 * 256);//Стоимость услуг по прайсу
                sheet.SetColumnWidth(15, 30 * 256);//Стоимость в агентском отчете
                sheet.SetColumnWidth(16, 30 * 256);//Номер агентского отчета
                sheet.SetColumnWidth(17, 30 * 256);//Сумма согласованной компенсации
                sheet.SetColumnWidth(18, 30 * 256);//Дата оплаты компенсации
                sheet.SetColumnWidth(19, 30 * 256);//Выдано из кассы
                sheet.SetColumnWidth(20, 30 * 256);//К оплате монтажнику
                sheet.SetColumnWidth(21, 15 * 256);//Доплаты мастеру
                sheet.SetColumnWidth(22, 15 * 256);//Статус оплаты монтажнику
                sheet.SetColumnWidth(23, 15 * 256);//Тип двери
                sheet.SetColumnWidth(24, 15 * 256);//Кол-во доп услуг
                sheet.SetColumnWidth(25, 30 * 256);//Претензия
                sheet.SetColumnWidth(26, 30 * 256);//Комментарий
                sheet.SetColumnWidth(27, 30 * 256);//Оценка
                sheet.SetColumnWidth(28, 25 * 256);//Комментарий клиента
                sheet.SetColumnWidth(29, 30 * 256);//Производитель
                sheet.SetColumnWidth(30, 30 * 256);//Комментарий оператора

                var fileName = Guid.NewGuid().ToString() + ".xls";
                var filePath = Path.Combine(UploadPath.TempDir, fileName);
                FileStream file = new FileStream(filePath, FileMode.Create); //CreateNew if a file exists, it will throw an exception

                hssfworkbook.Write(file);
                file.Close();

                return fileName;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ExcelUtils_Exception: " + ex.Message);
                return string.Empty;
            }
        }

        public static string Clients(string sheetName, List<string> headers, List<Client> values)
        {
            try
            {
                HSSFWorkbook hssfworkbook = new HSSFWorkbook();

                //
                DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
                dsi.Company = "";
                hssfworkbook.DocumentSummaryInformation = dsi;

                SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
                si.Subject = "";
                hssfworkbook.SummaryInformation = si;
                //

                //Вставить хотябы 1 лист в противном случае Excel сообщит «данные потеряны в файле
                var sheet = hssfworkbook.CreateSheet(sheetName);
                ((HSSFSheet)hssfworkbook.GetSheetAt(0)).AlternativeFormula = false;
                ((HSSFSheet)hssfworkbook.GetSheetAt(0)).AlternativeExpression = false;


                //Styles
                ICellStyle myHeaderStyle = hssfworkbook.CreateCellStyle();
                myHeaderStyle.ShrinkToFit = true;
                myHeaderStyle.WrapText = true;
                myHeaderStyle.Alignment = HorizontalAlignment.Center;
                myHeaderStyle.VerticalAlignment = VerticalAlignment.Center;
                //myHeaderStyle.BorderTop = BorderStyle.Medium;
                //myHeaderStyle.BorderBottom= BorderStyle.Medium;
                //myHeaderStyle.BorderLeft = BorderStyle.Medium;
                //myHeaderStyle.BorderRight = BorderStyle.Medium;
                //Шрифт заголовков
                var headerFont = hssfworkbook.CreateFont();
                headerFont.FontHeightInPoints = 12;
                headerFont.FontName = "Arial";
                headerFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
                myHeaderStyle.SetFont(headerFont);


                ICellStyle myStyle = hssfworkbook.CreateCellStyle();
                myStyle.ShrinkToFit = true;
                myStyle.WrapText = true;
                myStyle.Alignment = HorizontalAlignment.Center;
                myStyle.VerticalAlignment = VerticalAlignment.Center;
                //Шрифт
                var font = hssfworkbook.CreateFont();
                font.FontHeightInPoints = 8;
                font.FontName = "Arial";
                font.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.None;
                myHeaderStyle.SetFont(font);
                //------------------------------------------------------

                //Заголовки
                IRow row = sheet.CreateRow(0);
                for (int i = 0; i < headers.Count; i++)
                {
                    ICell cell = row.CreateCell(i);
                    cell.SetCellValue(headers[i]);
                    cell.CellStyle = myHeaderStyle;
                    cell.CellStyle.SetFont(headerFont);
                }


                //Сами данные
                ICell mycell;
                int my_i = 1;
                foreach (var oneClient in values)
                {
                    row = sheet.CreateRow(my_i);
                    
                    //
                    mycell = row.CreateCell(0);
                    mycell.SetCellValue(oneClient.GetFullName());
                    mycell.CellStyle = myStyle;

                    //
                    mycell = row.CreateCell(1);
                    string result = oneClient.ClientPhone + Environment.NewLine;
                    if(oneClient.Contacts != null && oneClient.Contacts.Count > 0)
                    {
                        foreach (var contact in oneClient.Contacts)
                        {
                            result += contact.Phone + Environment.NewLine;
                        }
                    }
                    mycell.SetCellValue(result);
                    mycell.CellStyle = myStyle;

                    //
                    mycell = row.CreateCell(2);
                    if(oneClient.ClientType != null && !string.IsNullOrEmpty(oneClient.ClientType.TypeName))
                    mycell.SetCellValue(oneClient.ClientType.TypeName);
                    mycell.CellStyle = myStyle;

                    my_i++;
                }

                //Ширина столбцов
                sheet.SetColumnWidth(0, 60 * 256);
                sheet.SetColumnWidth(1, 60 * 256);
                sheet.SetColumnWidth(2, 30 * 256);//Контакты


                var fileName = Guid.NewGuid().ToString() + ".xls";
                var filePath = Path.Combine(UploadPath.TempDir, fileName);
                FileStream file = new FileStream(filePath, FileMode.Create); //CreateNew if a file exists, it will throw an exception

                hssfworkbook.Write(file);
                file.Close();

                return fileName;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ExcelUtils_Exception: " + ex.Message);
                return string.Empty;
            }
        }

        public static string Employees(string sheetName, List<string> headers, List<Employee> values)
        {
            try
            {
                HSSFWorkbook hssfworkbook = new HSSFWorkbook();

                //
                DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
                dsi.Company = "";
                hssfworkbook.DocumentSummaryInformation = dsi;

                SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
                si.Subject = "";
                hssfworkbook.SummaryInformation = si;
                //

                //Вставить хотябы 1 лист в противном случае Excel сообщит «данные потеряны в файле
                var sheet = hssfworkbook.CreateSheet(sheetName);
                ((HSSFSheet)hssfworkbook.GetSheetAt(0)).AlternativeFormula = false;
                ((HSSFSheet)hssfworkbook.GetSheetAt(0)).AlternativeExpression = false;


                //Styles
                ICellStyle myHeaderStyle = hssfworkbook.CreateCellStyle();
                myHeaderStyle.ShrinkToFit = true;
                myHeaderStyle.WrapText = true;
                myHeaderStyle.Alignment = HorizontalAlignment.Center;
                myHeaderStyle.VerticalAlignment = VerticalAlignment.Center;
                //myHeaderStyle.BorderTop = BorderStyle.Medium;
                //myHeaderStyle.BorderBottom= BorderStyle.Medium;
                //myHeaderStyle.BorderLeft = BorderStyle.Medium;
                //myHeaderStyle.BorderRight = BorderStyle.Medium;
                //Шрифт заголовков
                var headerFont = hssfworkbook.CreateFont();
                headerFont.FontHeightInPoints = 12;
                headerFont.FontName = "Arial";
                headerFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
                myHeaderStyle.SetFont(headerFont);


                ICellStyle myStyle = hssfworkbook.CreateCellStyle();
                myStyle.ShrinkToFit = true;
                myStyle.WrapText = true;
                myStyle.Alignment = HorizontalAlignment.Center;
                myStyle.VerticalAlignment = VerticalAlignment.Center;
                //Шрифт
                var font = hssfworkbook.CreateFont();
                font.FontHeightInPoints = 8;
                font.FontName = "Arial";
                font.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.None;
                myHeaderStyle.SetFont(font);
                //------------------------------------------------------

                //Заголовки
                IRow row = sheet.CreateRow(0);
                for (int i = 0; i < headers.Count; i++)
                {
                    ICell cell = row.CreateCell(i);
                    cell.SetCellValue(headers[i]);
                    cell.CellStyle = myHeaderStyle;
                    cell.CellStyle.SetFont(headerFont);
                }


                //Сами данные
                ICell mycell;
                int my_i = 1;
                foreach (var oneEmployee in values)
                {
                    row = sheet.CreateRow(my_i);

                    //
                    mycell = row.CreateCell(0);
                    mycell.SetCellValue(oneEmployee.GetFullName());
                    mycell.CellStyle = myStyle;

                    //
                    mycell = row.CreateCell(1);
                    mycell.SetCellValue(oneEmployee.Phone);
                    mycell.CellStyle = myStyle;

                    //
                    mycell = row.CreateCell(2);
                    string result = string.Empty;
                    if(oneEmployee.Skills.Count > 0)
                    {
                        foreach (var skill in oneEmployee.Skills)
                        {
                            result+=skill.SkillName + Environment.NewLine;
                        }
                    }
                    mycell.SetCellValue(result);
                    mycell.CellStyle = myStyle;

                    //Регионы
                    mycell = row.CreateCell(3);
                    string result_r = string.Empty;
                    if (oneEmployee.Regions != null && oneEmployee.Regions.Count > 0)
                    {
                        foreach (var region in oneEmployee.Regions)
                        {
                            result_r += region.RegionName + Environment.NewLine;
                        }
                    }
                    mycell.SetCellValue(result_r);
                    mycell.CellStyle = myStyle;

                    my_i++;
                }


                    
                

                //Ширина столбцов
                sheet.SetColumnWidth(0, 60 * 256);
                sheet.SetColumnWidth(1, 60 * 256);
                sheet.SetColumnWidth(2, 60 * 256);
                sheet.SetColumnWidth(3, 60 * 256);

                var fileName = Guid.NewGuid().ToString() + ".xls";
                var filePath = Path.Combine(UploadPath.TempDir, fileName);
                FileStream file = new FileStream(filePath, FileMode.Create); //CreateNew if a file exists, it will throw an exception

                hssfworkbook.Write(file);
                file.Close();

                return fileName;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ExcelUtils_Exception: " + ex.Message);
                return string.Empty;
            }
        }

        public static string Orders(string sheetName, List<string> headers, List<OrderGridView> values)
        {
            try
            {
                HSSFWorkbook hssfworkbook = new HSSFWorkbook();

                //
                DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
                dsi.Company = "";
                hssfworkbook.DocumentSummaryInformation = dsi;

                SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
                si.Subject = "";
                hssfworkbook.SummaryInformation = si;
                //

                //Вставить хотябы 1 лист в противном случае Excel сообщит «данные потеряны в файле
                var sheet = hssfworkbook.CreateSheet(sheetName);
                ((HSSFSheet)hssfworkbook.GetSheetAt(0)).AlternativeFormula = false;
                ((HSSFSheet)hssfworkbook.GetSheetAt(0)).AlternativeExpression = false;


                //Styles
                ICellStyle myHeaderStyle = hssfworkbook.CreateCellStyle();
                myHeaderStyle.ShrinkToFit = true;
                myHeaderStyle.WrapText = true;
                myHeaderStyle.Alignment = HorizontalAlignment.Center;
                myHeaderStyle.VerticalAlignment = VerticalAlignment.Center;
                //myHeaderStyle.BorderTop = BorderStyle.Medium;
                //myHeaderStyle.BorderBottom= BorderStyle.Medium;
                //myHeaderStyle.BorderLeft = BorderStyle.Medium;
                //myHeaderStyle.BorderRight = BorderStyle.Medium;
                //Шрифт заголовков
                var headerFont = hssfworkbook.CreateFont();
                headerFont.FontHeightInPoints = 12;
                headerFont.FontName = "Arial";
                headerFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
                myHeaderStyle.SetFont(headerFont);


                ICellStyle myStyle = hssfworkbook.CreateCellStyle();
                myStyle.ShrinkToFit = true;
                myStyle.WrapText = true;
                myStyle.Alignment = HorizontalAlignment.Center;
                myStyle.VerticalAlignment = VerticalAlignment.Center;
                //Шрифт
                var font = hssfworkbook.CreateFont();
                font.FontHeightInPoints = 8;
                font.FontName = "Arial";
                font.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.None;
                myHeaderStyle.SetFont(font);
                //------------------------------------------------------

                //Заголовки
                IRow row = sheet.CreateRow(0);
                for (int i = 0; i < headers.Count; i++)
                {
                    ICell cell = row.CreateCell(i);
                    cell.SetCellValue(headers[i]);
                    cell.CellStyle = myHeaderStyle;
                    cell.CellStyle.SetFont(headerFont);
                }

                /*

                //Сами данные
                ICell mycell;
                int my_i = 1;
                foreach (var oneOrder in values)
                {
                    row = sheet.CreateRow(my_i);

                    //
                    mycell = row.CreateCell(0);
                    if(oneOrder.Order.OrderNum != null)
                        mycell.SetCellValue(oneOrder.Order.OrderNum.ToString());
                    mycell.CellStyle = myStyle;
                    sheet.SetColumnWidth(0, 30 * 256);

                    //
                    mycell = row.CreateCell(1);
                    if (oneOrder.Order.OrderName != null)
                        mycell.SetCellValue(oneOrder.Order.OrderName.ToString());
                    mycell.CellStyle = myStyle;
                    sheet.SetColumnWidth(1, 30 * 256);

                    //status
                    mycell = row.CreateCell(2);
                    if (oneOrder.Order.OrderStatus != null)
                        mycell.SetCellValue(oneOrder.Order.OrderStatus.StatusName.ToString());
                    mycell.CellStyle = myStyle;
                    sheet.SetColumnWidth(2, 30 * 256);

                    //Шаблон
                    mycell = row.CreateCell(3);
                    if (oneOrder.Order.Template != null)
                        mycell.SetCellValue(oneOrder.Order.Template.OrderTemplateName.ToString());
                    mycell.CellStyle = myStyle;
                    sheet.SetColumnWidth(3, 60 * 256);

                    //Тип заказа
                    mycell = row.CreateCell(4);
                    if (oneOrder.Order.Template.OrderType != null)
                        mycell.SetCellValue(oneOrder.Order.Template.OrderType.ToString());
                    mycell.CellStyle = myStyle;
                    sheet.SetColumnWidth(4, 60 * 256);

                    //Описание заказа
                    mycell = row.CreateCell(5);
                    if (oneOrder.Order.Template.OrderDescription != null)
                        mycell.SetCellValue(oneOrder.Order.Template.OrderDescription.ToString());
                    mycell.CellStyle = myStyle;
                    sheet.SetColumnWidth(5, 60 * 256);

                    //Адрес
                    mycell = row.CreateCell(6);
                    if (oneOrder.Order.Address != null && !string.IsNullOrEmpty(oneOrder.Order.Address.FullAddress))
                        mycell.SetCellValue(oneOrder.Order.Address.FullAddress.ToString());
                    mycell.CellStyle = myStyle;
                    sheet.SetColumnWidth(6, 60 * 256);

                    //Исполнитель
                    mycell = row.CreateCell(7);
                    if (oneOrder.Order.OrderEmployeeExecutor != null && oneOrder.Order.OrderEmployeeExecutor.Phone != null)
                        mycell.SetCellValue(oneOrder.Order.OrderEmployeeExecutor.GetFullName() + " " + oneOrder.Order.OrderEmployeeExecutor.Phone);
                    mycell.CellStyle = myStyle;
                    sheet.SetColumnWidth(7, 60 * 256);

                    //Контакты
                    mycell = row.CreateCell(8);
                    var v8 = oneOrder.Order.Contacts;
                    string v8_val = string.Empty;
                    if (v8 != null && v8.Count > 0)
                    {
                        foreach (ClientContact contact in v8)
                        {
                            v8_val += contact.ClientContactName + ": " + contact.Phone + Environment.NewLine;
                        }
                        
                        mycell.SetCellValue(v8_val.ToString());
                        mycell.CellStyle = myStyle;
                        sheet.SetColumnWidth(8, 60 * 256);
                    }

                    //Планируемая дата
                    mycell = row.CreateCell(9);
                    if (oneOrder.Order.PlannedTime != null)
                        mycell.SetCellValue(oneOrder.Order.PlannedTime.ToString());
                    mycell.CellStyle = myStyle;
                    sheet.SetColumnWidth(9, 30 * 256);

                    //Поля(Диспетчера) из шаблона
                    int cellCounter=10;
                    foreach(var field in oneOrder.Fields) 
                    {

                        mycell = row.CreateCell(cellCounter);
                        if(field != null)
                        { 
                            if (field.IsNumeric)
                            {
                                mycell.SetCellValue(field.FieldValueAsNumeric);
                                mycell.SetCellType(CellType.Numeric);
                            }
                            else
                            {
                                mycell.SetCellValue(field.FieldValueAsString);
                            }
                        }

                        mycell.CellStyle = myStyle;
                        sheet.SetColumnWidth(cellCounter, 40 * 256);

                        cellCounter++;
                    }

                    my_i++;
                }

                */

                var fileName = Guid.NewGuid().ToString() + ".xls";
                var filePath = Path.Combine(UploadPath.TempDir, fileName);
                FileStream file = new FileStream(filePath, FileMode.Create); //CreateNew if a file exists, it will throw an exception

                hssfworkbook.Write(file);
                file.Close();

                return fileName;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ExcelUtils_Exception: " + ex.Message);
                return string.Empty;
            }
        }



    }
}
