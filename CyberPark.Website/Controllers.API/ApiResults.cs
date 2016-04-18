using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CyberPark.Website.ViewModels;
namespace CyberPark.Website.Controllers.API
{
    public class ApiResult<T>
    {
        public bool Success { get; set; } = true;
        public string Message { get; set; }
        public Pager Pager { get; set; }
        public T Data { get; set; }
    }
}