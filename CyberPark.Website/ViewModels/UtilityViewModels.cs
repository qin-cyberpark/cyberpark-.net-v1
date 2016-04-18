using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CyberPark.Website.ViewModels
{
    public class Pager
    {
        public const int DefaultPageSize = 50;
        public Pager(int totalRecord, int pageSize = DefaultPageSize)
        {
            TotalRecord = totalRecord > 0 ? totalRecord : 0;
            PageSize = pageSize > 0 ? pageSize : 50;
            TotalPage = totalRecord / pageSize + (totalRecord % pageSize > 0 ? 1 : 0);
        }
        private int _page = 0;
        public int Page
        {
            get
            {
                return _page;
            }
            set
            {
                _page = value > 0 ? (value < TotalPage ? value : TotalPage) : (TotalPage > 0 ? 1 : 0);
            }
        }
        public int TotalRecord { get; private set; }
        public int PageSize { get; private set; }
        public int TotalPage { get; private set; }

        public static IList<T> GetPage<T>(IList<T> data, int page, int pageSize = DefaultPageSize)
        {
            if (data == null || data.Count == 0)
            {
                return data;
            }

            var pager = new Pager(data.Count, pageSize)
            {
                Page = page
            };

            return data.Skip(pager.PageSize * (pager.Page - 1)).Take(pager.PageSize).ToList();
        }
    }
}