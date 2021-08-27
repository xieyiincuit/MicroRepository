using System;
using System.Collections.Generic;
using System.Linq;

namespace XieyiESLibrary.Config
{
    /// <summary>
    ///     ES basic config
    /// </summary>
    public class ESConfig
    {
        /// <summary>
        ///     ElasticSearch ip address string
        /// </summary>
        public string Urls { get; set; }

        /// <summary>
        ///     Authentication Name
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        ///     Authentication Password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        ///     if you have many node url，split they with '|'
        /// </summary>
        public List<Uri> Uris => Urls.Split('|').Select(x => new Uri(x)).ToList();
    }
}