using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyArchitect.EntityFrameworkCore.Exts
{
    /// <summary>
    /// 簡單支援 DbParameters 所需要參數傳遞
    /// </summary>
    public class DbParames
    {
        /// <summary>
        /// 參數名稱
        /// </summary>
        public string DbParameName { get; set; }
        /// <summary>
        /// 參數值內容
        /// </summary>
        public object DbParameValue { get; set; }
        /// <summary>
        /// 參數型態
        /// </summary>
        public DbType DbParameType { get; set; }
    }
}
