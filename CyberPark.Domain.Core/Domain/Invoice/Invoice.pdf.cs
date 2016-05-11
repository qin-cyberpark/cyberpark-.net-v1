using CyberPark.Domain.Utilities;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberPark.Domain.Core
{
    public partial class Invoice
    {
        //color
        private static BaseColor lightGreen = new BaseColor(92, 184, 92);
        private static Font ftSm = FontFactory.GetFont("Helvetica Neue", 10, Font.NORMAL, BaseColor.BLACK);
        private static Font ftNormal = FontFactory.GetFont("Helvetica Neue", 12, Font.NORMAL, BaseColor.BLACK);
        private static Font ftNormalWhitle = FontFactory.GetFont("Helvetica Neue", 12, Font.NORMAL, BaseColor.WHITE);
        private static Font ftBig = FontFactory.GetFont("Helvetica Neue", 14, Font.BOLD, BaseColor.BLACK);
        private static Font ftBiger = FontFactory.GetFont("Helvetica Neue", 16, Font.BOLD, BaseColor.BLACK);
        private static Font ftSign = FontFactory.GetFont("Helvetica Neue", 30, Font.BOLD, BaseColor.BLACK);
        private static Font ftSmWhite = FontFactory.GetFont("Helvetica Neue", 10, Font.NORMAL, BaseColor.WHITE);
        private static Font ftBgWhite = FontFactory.GetFont("Helvetica Neue", 28, Font.BOLD, BaseColor.WHITE);

        private static int side_margin = 30;
        private static int top_margin = 30;


        class InvPageEvent : PdfPageEventHelper
        {

            private Invoice _inv = null;

            public InvPageEvent(Invoice inv)
            {
                _inv = inv;
            }
            public override void OnStartPage(PdfWriter writer, Document doc)
            {
                base.OnStartPage(writer, doc);

                PdfContentByte context = writer.DirectContent;
                var ct = new ColumnText(context);

                context.Rectangle(0, doc.PageSize.Height - 10, doc.PageSize.Width, 10);
                context.SetColorStroke(lightGreen);
                context.SetColorFill(lightGreen);
                context.FillStroke();

                //logo
                var logo = Image.GetInstance(SysConfig.Instance.InvoicePdfDirectory + "logo.png");
                logo.ScaleToFit(150, 60);
                logo.SetAbsolutePosition(side_margin, doc.PageSize.Height - 50 - top_margin);
                context.AddImage(logo);

                //caption
                var statementInfo = new Phrase("Statement / Tax Invoice", ftBig);
                ColumnText.ShowTextAligned(context, Element.ALIGN_LEFT, statementInfo, side_margin, doc.PageSize.Height - 65 - top_margin, 0);

                var gstInfo = new Phrase("GST Registration Number: 113-460-148", ftBig);
                ColumnText.ShowTextAligned(context, Element.ALIGN_LEFT, gstInfo, side_margin, doc.PageSize.Height - 80 - top_margin, 0);


                //contact info
                var contactL1 = new Phrase("CyberPark Limited", ftSm);
                var contactL2 = new Phrase("PO Box 41547, St Lukes, Auckland 1346, New Zealand", ftSm);
                var contactL3 = new Phrase("Tel: 0800 2 CYBER (29237)", ftSm);
                var contactL4 = new Phrase("www.cyberpark.co.nz", ftSm);
                ColumnText.ShowTextAligned(context, Element.ALIGN_RIGHT, contactL1, doc.PageSize.Width - side_margin, doc.PageSize.Height - 40 - top_margin, 0);
                ColumnText.ShowTextAligned(context, Element.ALIGN_RIGHT, contactL2, doc.PageSize.Width - side_margin, doc.PageSize.Height - 52 - top_margin, 0);
                ColumnText.ShowTextAligned(context, Element.ALIGN_RIGHT, contactL3, doc.PageSize.Width - side_margin, doc.PageSize.Height - 64 - top_margin, 0);
                ColumnText.ShowTextAligned(context, Element.ALIGN_RIGHT, contactL4, doc.PageSize.Width - side_margin, doc.PageSize.Height - 76 - top_margin, 0);

                //hr
                context.Rectangle(side_margin, doc.PageSize.Height - 90 - top_margin, doc.PageSize.Width - side_margin * 2, 3);
                context.SetColorStroke(lightGreen);
                context.SetColorFill(lightGreen);
                context.FillStroke();

                //name
                var name = new Phrase(_inv.Account.Name, ftBig);
                ct.SetSimpleColumn(name, side_margin, doc.PageSize.Height - 90 - top_margin, 300, 30, 20f, Element.ALIGN_LEFT | Element.ALIGN_TOP);
                ct.Go();

                //address
                var address = new Phrase(_inv.Account.Address, ftBig);
                ct.SetSimpleColumn(address, side_margin, doc.PageSize.Height - 110 - top_margin, 300, 30, 20f, Element.ALIGN_LEFT | Element.ALIGN_TOP);
                ct.Go();

                //invoice Id
                var invId = new Phrase(new Chunk("Invoice Id: ", ftNormal));
                invId.Add(new Chunk(_inv.Id.ToString(), ftBig));
                ColumnText.ShowTextAligned(context, Element.ALIGN_RIGHT, invId, doc.PageSize.Width - side_margin, doc.PageSize.Height - 135, 0);

                //customer Id
                var custId = new Phrase(new Chunk(string.Format("Customer Id: {0:dd MMM yyyy}", _inv.Account.CustomerId.ToString(), ftNormal)));
                ColumnText.ShowTextAligned(context, Element.ALIGN_RIGHT, custId, doc.PageSize.Width - side_margin, doc.PageSize.Height - 150, 0);

                //invoice date
                var invDate = new Phrase(new Chunk(string.Format("Date: {0:dd MMM yyyy}", _inv.IssuedDate), ftNormal));
                ColumnText.ShowTextAligned(context, Element.ALIGN_RIGHT, invDate, doc.PageSize.Width - side_margin, doc.PageSize.Height - 165, 0);



            }
            public override void OnEndPage(PdfWriter writer, Document document)
            {
                //footer
                PdfContentByte context = writer.DirectContent;
                context.Rectangle(0, 0, document.PageSize.Width, 10);
                context.SetColorStroke(lightGreen);
                context.SetColorFill(lightGreen);
                context.FillStroke();
            }
        }


        public string PdfPath
        {
            get
            {
                return string.Format("{0}inv_{1}.pdf", SysConfig.Instance.InvoicePdfDirectory, Id);
            }
        }
        //public 
        public void ToPDF()
        {
            Logger.Info("ToPDF", string.Format("to pdf {0}", PdfPath));
            FileStream fs = new FileStream(PdfPath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            Document doc = new Document(new Rectangle(PageSize.A4), side_margin, side_margin, 200, 30);
            PdfWriter writer = PdfWriter.GetInstance(doc, fs);
            writer.PageEvent = new InvPageEvent(this);
            doc.Open();

            PdfContentByte context = writer.DirectContent;
            var ct = new ColumnText(context);
            #region balance calculation
            //Your Bill

            //previous balance
            context.Rectangle(side_margin, doc.PageSize.Height - top_margin - 270, 100, 50);
            context.SetColorStroke(lightGreen);
            context.SetColorFill(lightGreen);
            context.FillStroke();

            ct.SetSimpleColumn(new Phrase("Previous Balance", ftSmWhite),
                                side_margin, doc.PageSize.Height - side_margin - 230, side_margin + 100, 50, 0, Element.ALIGN_CENTER);
            ct.Go();

            ct.SetSimpleColumn(new Phrase(PreviousBalance.ToString("#,##0.00"), ftBgWhite),
                     side_margin, doc.PageSize.Height - side_margin - 230, side_margin + 100, 50, 30, Element.ALIGN_CENTER);
            ct.Go();

            //-
            ct.SetSimpleColumn(new Phrase(PreviousBalance.ToString("-"), ftSign),
                            side_margin + 100, doc.PageSize.Height - side_margin - 230, side_margin + 130, 50, 30, Element.ALIGN_CENTER);
            ct.Go();

            //current amount
            context.Rectangle(side_margin + 130, doc.PageSize.Height - side_margin - 270, 100, 50);
            context.SetColorStroke(lightGreen);
            context.SetColorFill(lightGreen);
            context.FillStroke();

            ct.SetSimpleColumn(new Phrase("Current Amount", ftSmWhite),
                              side_margin + 130, doc.PageSize.Height - side_margin - 230, side_margin + 230, 50, 0, Element.ALIGN_CENTER);
            ct.Go();

            ct.SetSimpleColumn(new Phrase(ChargeAmountIncludeGST.ToString("#,##0.00"), ftBgWhite),
                     side_margin + 130, doc.PageSize.Height - side_margin - 230, side_margin + 230, 50, 30, Element.ALIGN_CENTER);
            ct.Go();

            //+
            ct.SetSimpleColumn(new Phrase(PreviousBalance.ToString("+"), ftSign),
                            side_margin + 230, doc.PageSize.Height - side_margin - 230, side_margin + 260, 50, 30, Element.ALIGN_CENTER);
            ct.Go();

            //topup & adjust
            context.Rectangle(side_margin + 260, doc.PageSize.Height - side_margin - 270, 100, 50);
            context.SetColorStroke(lightGreen);
            context.SetColorFill(lightGreen);
            context.FillStroke();

            ct.SetSimpleColumn(new Phrase("TopUp / Adjust", ftSmWhite),
                  side_margin + 260, doc.PageSize.Height - side_margin - 230, side_margin + 360, 50, 0, Element.ALIGN_CENTER);
            ct.Go();

            ct.SetSimpleColumn(new Phrase(((TransactionAmount - AdjustAmount)).ToString("#,##0.00"), ftBgWhite),
                     side_margin + 260, doc.PageSize.Height - side_margin - 230, side_margin + 360, 50, 30, Element.ALIGN_CENTER);
            ct.Go();

            //=
            ct.SetSimpleColumn(new Phrase(PreviousBalance.ToString("="), ftSign),
                            side_margin + 360, doc.PageSize.Height - side_margin - 230, side_margin + 390, 50, 30, Element.ALIGN_CENTER);
            ct.Go();

            //current balance
            context.Rectangle(side_margin + 390, doc.PageSize.Height - side_margin - 270, 120, 50);
            context.SetColorStroke(CurrentBalance > 0 ? lightGreen : BaseColor.RED);
            context.SetColorFill(CurrentBalance > 0 ? lightGreen : BaseColor.RED);
            context.FillStroke();

            ct.SetSimpleColumn(new Phrase("Current Balance", ftSmWhite),
                        side_margin + 390, doc.PageSize.Height - side_margin - 230, side_margin + 510, 50, 0, Element.ALIGN_CENTER);
            ct.Go();


            ct.SetSimpleColumn(new Phrase(CurrentBalance.ToString("#,##0.00"), ftBgWhite),
                        side_margin + 390, doc.PageSize.Height - side_margin - 230, side_margin + 510, 50, 30, Element.ALIGN_CENTER);
            ct.Go();
            #endregion

            #region balance detail
            //title:detail
            //current balance
            context.Rectangle(side_margin, doc.PageSize.Height - 350, doc.PageSize.Width - side_margin * 2, 12);
            context.SetColorStroke(lightGreen);
            context.SetColorFill(lightGreen);
            context.FillStroke();

            ct.SetSimpleColumn(new Phrase("Balance Details", ftNormalWhitle),
                        0, doc.PageSize.Height - 348, doc.PageSize.Width, 12, 0, Element.ALIGN_CENTER);
            ct.Go();

            //detail table
            var tbDetail = new PdfPTable(3);
            tbDetail.DefaultCell.Border = Rectangle.NO_BORDER;
            tbDetail.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;

            var cellWidths = new float[] { 260f, 190f, 70f };
            var tbHeadr = new string[] { "Product & Service", "Date", "inc GST" };
            tbDetail.SetTotalWidth(cellWidths);

            foreach (var txt in tbHeadr)
            {
                //table header
                var cell = new PdfPCell(new Phrase(txt, ftNormal));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Border = Rectangle.BOTTOM_BORDER;
                cell.PaddingBottom = 6f;
                cell.BorderWidthBottom = 2f;
                tbDetail.AddCell(cell);
            }

            //table: product
            foreach (var pdtRec in ProductCharges.OrderByDescending(x => x.AmountGSTExclusive))
            {
                //product name
                tbDetail.AddCell(new Phrase(pdtRec.Product.Name, ftNormal));

                var dt = new PdfPCell(new Phrase(string.Format("{0:dd MMM, yyyy} to {1:dd MMM, yyyy}",
                                                            pdtRec.PreviousProductChargedToDate.AddDays(1),
                                                            pdtRec.CurrentProductChargedToDate), ftNormal));
                dt.Border = Rectangle.NO_BORDER;
                dt.HorizontalAlignment = Rectangle.ALIGN_LEFT;
                tbDetail.AddCell(dt);

                //var amt = new PdfPCell(new Phrase(pdtRec.Product.PriceGSTExclusive.ToString("$#,##0.00"), ftNormal));
                //amt.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                //amt.Border = Rectangle.NO_BORDER;
                //tbDetail.AddCell(amt);

                var amt1 = new PdfPCell(new Phrase(pdtRec.Product.PriceGSTInclusive.ToString("$#,##0.00"), ftNormal));
                amt1.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                amt1.Border = Rectangle.NO_BORDER;
                tbDetail.AddCell(amt1);

                foreach (var srv in pdtRec.Product.Services)
                {
                    var pkgDtl = new PdfPCell(new Phrase(srv.Description, ftNormal));
                    pkgDtl.Border = Rectangle.NO_BORDER;
                    pkgDtl.PaddingLeft = 30f;
                    pkgDtl.Colspan = 3;
                    tbDetail.AddCell(pkgDtl);
                }
            }

            //table: addon
            foreach (var addon in AddonCharges)
            {
                tbDetail.AddCell(new Phrase(addon.Description, ftNormal));

                tbDetail.AddCell(new Phrase(string.Format("{0:dd MMM, yyyy} - {1:dd MMM, yyyy}",
                                                            addon.DateFrom, addon.DateTo), ftNormal));

                //var amt = new PdfPCell(new Phrase(addon.Charge.ToString("$#,##0.00"), ftNormal));
                //amt.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                //amt.Border = Rectangle.NO_BORDER;
                //tbDetail.AddCell(amt);

                var amt1 = new PdfPCell(new Phrase(addon.Charge.ToString("$#,##0.00"), ftNormal));
                amt1.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                amt1.Border = Rectangle.NO_BORDER;
                tbDetail.AddCell(amt1);
            }

            //table: calling
            foreach (var callChrg in CallingCharges)
            {
                if (callChrg.Charge > 0)
                {
                    tbDetail.AddCell(new Phrase(string.Format("Calling Charge - {0}", callChrg.Service.IdentityNumber, ftNormal)));
                    tbDetail.AddCell("");

                    //var amt = new PdfPCell(new Phrase(callChrg.Charge.ToString("$#,##0.00"), ftNormal));
                    //amt.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                    //amt.Border = Rectangle.NO_BORDER;
                    //tbDetail.AddCell(amt);

                    var amt1 = new PdfPCell(new Phrase(callChrg.Charge.ToString("$#,##0.00"), ftNormal));
                    amt1.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                    amt1.Border = Rectangle.NO_BORDER;
                    tbDetail.AddCell(amt1);
                }
            }



            //charge
            var chrg = new PdfPCell(new Phrase("Current Amount", ftNormal));
            chrg.Border = Rectangle.TOP_BORDER;
            chrg.PaddingTop = 2f;
            chrg.BorderWidthTop = 2f;
            chrg.Colspan = 2;
            tbDetail.AddCell(chrg);

            //var chrgAmt = new PdfPCell(new Phrase((-ChargeAmountExcludeGST).ToString("$#,##0.00"), ftNormal));
            //chrgAmt.HorizontalAlignment = Element.ALIGN_RIGHT;
            //chrgAmt.Border = Rectangle.TOP_BORDER;
            //chrgAmt.PaddingTop = 2f;
            //chrgAmt.BorderWidthTop = 2f;
            //tbDetail.AddCell(chrgAmt);

            var chrgAmt2 = new PdfPCell(new Phrase(ChargeAmountIncludeGST.ToString("$#,##0.00"), ftNormal));
            chrgAmt2.HorizontalAlignment = Element.ALIGN_RIGHT;
            chrgAmt2.Border = Rectangle.TOP_BORDER;
            chrgAmt2.PaddingTop = 2f;
            chrgAmt2.BorderWidthTop = 2f;
            tbDetail.AddCell(chrgAmt2);


            //previous balance
            var preBal = new PdfPCell(new Phrase("Previous Balance", ftNormal));
            preBal.Border = Rectangle.NO_BORDER;
            preBal.PaddingTop = 30f;
            preBal.Colspan = 2;
            tbDetail.AddCell(preBal);

            var preBalAmt = new PdfPCell(new Phrase(PreviousBalance.ToString("$#,##0.00"), ftNormal));
            preBalAmt.HorizontalAlignment = Element.ALIGN_RIGHT;
            preBalAmt.Border = Rectangle.NO_BORDER;
            preBalAmt.PaddingTop = 30f;
            tbDetail.AddCell(preBalAmt);

            //charge
            var currAmount = new PdfPCell(new Phrase("Current Amount", ftNormal));
            currAmount.Border = Rectangle.NO_BORDER;
            currAmount.Colspan = 2;
            tbDetail.AddCell(currAmount);

            //var chrgAmt = new PdfPCell(new Phrase((-ChargeAmountExcludeGST).ToString("$#,##0.00"), ftNormal));
            //chrgAmt.HorizontalAlignment = Element.ALIGN_RIGHT;
            //chrgAmt.Border = Rectangle.TOP_BORDER;
            //chrgAmt.PaddingTop = 2f;
            //chrgAmt.BorderWidthTop = 2f;
            //tbDetail.AddCell(chrgAmt);

            var currAmount2 = new PdfPCell(new Phrase((-ChargeAmountIncludeGST).ToString("$#,##0.00"), ftNormal));
            currAmount2.HorizontalAlignment = Element.ALIGN_RIGHT;
            currAmount2.Border = Rectangle.NO_BORDER;
            tbDetail.AddCell(currAmount2);

            //Transaction
            int idx = 0;
            foreach (var trans in Transactions)
            {
                PdfPCell title;
                if (idx++ == 0)
                {
                    title = new PdfPCell(new Phrase("Transaction", ftNormal));

                }
                else
                {
                    title = new PdfPCell(new Phrase(""));
                }
                title.Border = Rectangle.NO_BORDER;
                tbDetail.AddCell(title);

                tbDetail.AddCell(new Phrase(string.Format("{0:dd MMM, yyyy}", trans.Date), ftNormal));

                var tranAmount = new PdfPCell(new Phrase(trans.Amount.ToString("+$#,##0.00"), ftNormal));
                tranAmount.HorizontalAlignment = Element.ALIGN_RIGHT;
                tranAmount.Border = Rectangle.NO_BORDER;
                tbDetail.AddCell(tranAmount);
            }

            //adjust
            foreach (var adj in Adjustments)
            {
                var memo = new PdfPCell(new Phrase(adj.Memo, ftNormal));
                memo.Border = Rectangle.NO_BORDER;
                memo.Colspan = 2;
                tbDetail.AddCell(memo);

                var adjAmount = new PdfPCell(new Phrase(adj.Amount > 0 ? "+" : "-" + adj.Amount.ToString("$#,##0.00"), ftNormal));
                adjAmount.HorizontalAlignment = Element.ALIGN_RIGHT;
                adjAmount.Border = Rectangle.NO_BORDER;
                tbDetail.AddCell(adjAmount);
            }

            //current balance
            var currBal = new PdfPCell(new Phrase("Current Balance", ftNormal));
            currBal.HorizontalAlignment = Element.ALIGN_LEFT;
            currBal.Border = Rectangle.TOP_BORDER;
            currBal.BorderWidthTop = 2f;
            currBal.PaddingTop = 2f;
            currBal.Colspan = 2;
            tbDetail.AddCell(currBal);

            var currBalAmt = new PdfPCell(new Phrase(CurrentBalance.ToString("$#,##0.00"), ftNormal));
            currBalAmt.HorizontalAlignment = Element.ALIGN_RIGHT;
            currBalAmt.Border = Rectangle.TOP_BORDER;
            currBalAmt.BorderWidthTop = 2f;
            currBalAmt.PaddingTop = 2f;
            tbDetail.AddCell(currBalAmt);


            //draw table
            tbDetail.WriteSelectedRows(0, -1, side_margin, doc.PageSize.Height - 350, context);

            #endregion

            //new page
            doc.NewPage();

            //calling record
            foreach (var call in CallingCharges)
            {
                if (call.CallingRecords.Count > 0)
                {
                    doc.Add(new Phrase(call.Service.Description, ftBig));

                    //calling table
                    var tbCalling = new PdfPTable(6);
                    var callTableWidths = new float[] { 150f, 150f, 135f, 50f, 50f, 50f };
                    tbCalling.SetTotalWidth(callTableWidths);
                    tbCalling.WidthPercentage = 100;
                    tbCalling.HeaderRows = 1;
                    tbCalling.DefaultCell.Border = Rectangle.NO_BORDER;
                    tbCalling.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;

                    var callTbHeaders = new string[] { "Calling Start", "Destination", "Description", "Min.", "Fee", "Charge" };
                    foreach (var txt in callTbHeaders)
                    {
                        var cell = new PdfPCell(new Phrase(txt, ftNormal));
                        cell.Border = Rectangle.BOTTOM_BORDER;
                        cell.BorderWidthBottom = 2f;
                        cell.PaddingBottom = 6f;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        tbCalling.AddCell(cell);
                    }

                    foreach (var rec in call.CallingRecords.OrderBy(x => x.CallStart))
                    {
                        tbCalling.AddCell(new Phrase(rec.CallStart.ToString("dd MMM, yyyy HH:mm:ss"), ftNormal));
                        tbCalling.AddCell(new Phrase(rec.DesNumber, ftNormal));
                        tbCalling.AddCell(new Phrase(string.IsNullOrEmpty(rec.Description) ? rec.AreaName : rec.Description, ftNormal));

                        var min = new PdfPCell(new Phrase(rec.ChargeMinute.ToString(), ftNormal));
                        min.Border = Rectangle.NO_BORDER;
                        min.HorizontalAlignment = Element.ALIGN_RIGHT;
                        tbCalling.AddCell(min);

                        var fee = new PdfPCell(new Phrase(rec.Charge.ToString("$#,##0.00"), ftNormal));
                        fee.Border = Rectangle.NO_BORDER;
                        fee.HorizontalAlignment = Element.ALIGN_RIGHT;
                        tbCalling.AddCell(fee);

                        var charge = new PdfPCell(new Phrase(rec.ActualCharge.Value.ToString("$#,##0.00"), ftNormal));
                        charge.Border = Rectangle.NO_BORDER;
                        charge.HorizontalAlignment = Element.ALIGN_RIGHT;
                        tbCalling.AddCell(charge);
                    }

                    doc.Add(tbCalling);
                }

                //close
                doc.Close();

            }
        }
    }
}