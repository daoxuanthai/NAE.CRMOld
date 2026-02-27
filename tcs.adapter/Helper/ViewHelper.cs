using System;
using System.IO;
using System.Web;
using tcs.bo;
using WebMarkupMin.Core;

namespace tcs.adapter.Helper
{
    public static class ViewHelper
    {
        public static string MinifyHtml(string source)
        {
            try
            {
                var htmlMinifier = new HtmlMinifier();
                var result = htmlMinifier.Minify(source);
                if (result.Errors.Count == 0)
                {
                    return result.MinifiedContent;
                }
                return source;
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        #region Pager

        public static string GetTotalString(int intCurrentPage, int intPageSize, int intTotalRecord)
        {
            if (intTotalRecord == 0)
                return string.Empty;

            if (intTotalRecord < intPageSize)
                return string.Format("Hiển thị <b>{0}</b> đến <b>{1}</b>", 1, intTotalRecord);

            string strTemplate = "Hiển thị <b>{0}</b> đến <b>{1}</b> trong tổng số <b>{2}</b>";
            int intFromRecord = intCurrentPage * intPageSize + 1;
            int intToRecord = (intCurrentPage + 1) * intPageSize;
            if (intToRecord > intTotalRecord)
                intToRecord = intTotalRecord;

            return string.Format(strTemplate, intFromRecord, intToRecord, intTotalRecord);
        }

        public static string BuildPaging(int total, int currPage, int slidePageNum, int rowperPage)
        {
            int totalPage;
            if (total % rowperPage > 0)
                totalPage = (total / rowperPage) + 1;
            else
                totalPage = total / rowperPage;

            if (totalPage == 1)
                return string.Empty;

            string sql = "<div class='paging'>";
            if (currPage <= totalPage)
            {
                int pageNumber;
                int start;
                if (currPage == 1)
                {
                    pageNumber = slidePageNum;
                    if (pageNumber > totalPage) pageNumber = totalPage;
                    start = 1;
                }
                else
                {
                    sql = sql + " <a href='#' onclick='return ChangePage(\"1\")' title='Trang đầu'>‹</a>";
                    sql = sql + " <a href='#' onclick='return ChangePage(\"" + (currPage - 1) + "\");' title='Trang trước'>«</a>";
                    if ((totalPage - currPage) < (slidePageNum / 2))
                    {
                        start = (totalPage - slidePageNum) + 1;
                        if (start < 0) start = 1;
                        pageNumber = totalPage;
                    }
                    else
                    {
                        if (currPage - (slidePageNum / 2) == 0)
                        {
                            start = 1;
                            pageNumber = currPage + (slidePageNum / 2) + 1;
                            if (totalPage < pageNumber) pageNumber = totalPage;
                        }
                        else
                        {
                            start = currPage - (slidePageNum / 2);
                            if (start < 0) start = 1;
                            pageNumber = currPage + (slidePageNum / 2);
                            if (totalPage < pageNumber) pageNumber = totalPage;
                            else
                                if (pageNumber < slidePageNum) pageNumber = slidePageNum;
                        }
                    }
                }
                int i = start;
                while (i <= pageNumber)
                {
                    if (i == currPage) sql = sql + " <span>" + i + "</span> ";
                    else
                        sql = sql + " <a href='' onclick='return ChangePage(\"" + i + "\");'>" + i + "</a>";
                    i++;
                }
                if (currPage < totalPage)
                {
                    sql = sql + " <a href='#' onclick='return ChangePage(\"" + (currPage + 1) + "\");' title='Trang sau'>»</a>";
                    sql = sql + " <a href='#' onclick='return ChangePage(\"" + totalPage + "\");' title='Trang cuối'>›</a>";
                }
            }
            sql = sql + "</div>";
            return sql;
        }

        public static string BuildPaging(string strUrl, int Total, int CurrPage, int PageSize, int RowperPage)
        {
            string SQL = string.Empty;
            try
            {
                int PageNumber = 1;
                int i = 1;
                int TotalPage;
                if (Total % RowperPage > 0)
                    TotalPage = (Total / RowperPage) + 1;
                else
                    TotalPage = Total / RowperPage;
                if (TotalPage <= 1)
                    return string.Empty;
                int Start = 0;
                SQL = "<div class='paging'>";
                if (CurrPage <= TotalPage)
                {
                    if (CurrPage == 1)
                    {
                        PageNumber = PageSize;
                        if (PageNumber > TotalPage) PageNumber = TotalPage;
                        Start = 1;
                    }
                    else
                    {
                        SQL = SQL + " <a href='" + GenPagingUrl(strUrl, 1) + "' title='Trang đầu'>‹</a>";
                        SQL = SQL + " <a href='" + GenPagingUrl(strUrl, CurrPage - 1) + "' title='Trang trước'>«</a>";
                        if ((TotalPage - CurrPage) < (PageSize / 2))
                        {
                            Start = (TotalPage - PageSize) + 1;
                            if (Start <= 0) Start = 1;
                            PageNumber = TotalPage;
                        }
                        else
                        {
                            if (CurrPage - (PageSize / 2) == 0)
                            {
                                Start = 1;
                                PageNumber = CurrPage + (PageSize / 2) + 1;
                                if (TotalPage < PageNumber) PageNumber = TotalPage;
                            }
                            else
                            {
                                Start = CurrPage - (PageSize / 2);
                                if (Start <= 0) Start = 1;
                                PageNumber = CurrPage + (PageSize / 2);
                                if (TotalPage < PageNumber) PageNumber = TotalPage;
                                else
                                    if (PageNumber < PageSize) PageNumber = PageSize;
                            }
                        }
                    }
                    i = Start;
                    while (i <= PageNumber)
                    {
                        if (i == CurrPage) SQL = SQL + " <span>" + i + "</span> ";
                        else
                            SQL = SQL + " <a href='" + GenPagingUrl(strUrl, i) + "'>" + i + "</a>";
                        i++;
                    }
                    if (CurrPage < TotalPage)
                    {
                        SQL = SQL + " <a href='" + GenPagingUrl(strUrl, CurrPage + 1) + "' title='Trang sau'>»</a>";
                        SQL = SQL + " <a href='" + GenPagingUrl(strUrl, TotalPage) + "' title='Trang cuối'>›</a>";
                    }
                }
                SQL = SQL + "</div>";
            }
            catch (Exception ex)
            {
                SQL = string.Empty;
            }
            return SQL;
        }

        public static string BuildCMSPaging(int total, int currPage, int slidePageNum, int rowperPage)
        {
            int totalPage;
            if (total % rowperPage > 0)
                totalPage = (total / rowperPage) + 1;
            else
                totalPage = total / rowperPage;

            if (totalPage == 1)
                return string.Empty;

            string sql = "<div class='dataTables_paginate paging_bootstrap_alt pagination'><ul>";
            if (currPage < totalPage)
            {
                int pageNumber;
                int start;
                if (currPage == 0)
                {
                    pageNumber = slidePageNum;
                    if (pageNumber >= totalPage) pageNumber = totalPage;
                    start = 0;
                }
                else
                {
                    sql = sql + " <li class='first'><a onclick='return ChangePage(0)' href='#'>Đầu</a></li>";
                    sql = sql + " <li class='prev'><a onclick='return ChangePage(" + (currPage - 1) + ")' href='#'>Trước</a></li>";
                    if ((totalPage - currPage) < (slidePageNum / 2))
                    {
                        start = (totalPage - slidePageNum) + 1;
                        if (start < 0) start = 0;
                        pageNumber = totalPage;
                    }
                    else
                    {
                        if (currPage - (slidePageNum / 2) == 0)
                        {
                            start = 0;
                            pageNumber = currPage + (slidePageNum / 2) + 1;
                            if (totalPage <= pageNumber) pageNumber = totalPage;
                        }
                        else
                        {
                            start = currPage - (slidePageNum / 2);
                            if (start < 0) start = 0;
                            pageNumber = currPage + (slidePageNum / 2);
                            if (totalPage <= pageNumber) pageNumber = totalPage;
                            else
                                if (pageNumber < slidePageNum) pageNumber = slidePageNum;
                        }
                    }
                }
                int i = start;
                while (i < pageNumber)
                {
                    if (i == currPage) sql = sql + " <li class='active'><a>" + (i + 1) + "</a></li> ";
                    else
                        sql = sql + " <li><a onclick='return ChangePage(" + i + ")' href='#'>" + (i + 1) + "</a></li>";
                    i++;
                }
                if (currPage < totalPage - 1)
                {
                    sql = sql + " <li class='next'><a onclick='return ChangePage(" + (currPage + 1) + ")' href='#'>Sau</a></li>";
                    sql = sql + " <li class='last'><a onclick='return ChangePage(" + (totalPage - 1) + ")' href='#'>Cuối</a></li>";
                }
            }
            sql = sql + "</div>";
            return sql;
        }
        
        private static string GenPagingUrl(string strUrl, int intCurrentPage)
        {
            if (string.IsNullOrEmpty(strUrl))
                return strUrl;
            var pageUrl = intCurrentPage > 1 ? (strUrl.Contains("?") ? "&" : "?") + "p=" + intCurrentPage.ToString() : string.Empty;
            return string.Concat(strUrl, pageUrl);
        }

        #endregion

        public static bool IsContinueCare(int status)
        {
            return status == CustomerStatus.NotCaring.Key || status == CustomerStatus.ContinueCare.Key || 
                status == CustomerStatus.Potential.Key || status == CustomerStatus.MaybeContract.Key;
        }

        public static bool IsValidFileUpload(string fileExtension, string validFileTypes)
        {
            if (string.IsNullOrEmpty(fileExtension) || string.IsNullOrEmpty(validFileTypes))
                return false;

            return validFileTypes.Contains(fileExtension.ToLower());
        }

        public static bool UploadFile(string filePath, string fileName, HttpPostedFileBase file)
        {
            try
            {
                if (!Directory.Exists(filePath))
                    Directory.CreateDirectory(filePath);

                string strAbsolutePath = Path.Combine(filePath, fileName);

                file.SaveAs(strAbsolutePath);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return false;
        }

        public static string GetPostTime(DateTime postDate, int showDateFrom = 6)
        {
            TimeSpan ts = DateTime.Now - postDate;
            if (ts.TotalMinutes < 60)
                return string.Format("{0} phút trước", ts.TotalMinutes.ToString("#,##"));
            if (ts.TotalHours < 24)
                return string.Format("{0} giờ trước", ts.TotalHours.ToString("#,##"));
            if (ts.TotalDays < showDateFrom)
                return string.Format("{0} ngày trước", ts.TotalDays.ToString("#,##"));
            else
            {
                return postDate.ToString("dd/MM/yyyy HH:mm");
            }
        }
    }
}
