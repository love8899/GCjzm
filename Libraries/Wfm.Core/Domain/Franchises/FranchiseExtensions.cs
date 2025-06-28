using System;
using System.Collections.Generic;
using System.Linq;

namespace Wfm.Core.Domain.Franchises
{
    public static class FranchiseExtensions
    {
        /// <summary>
        /// Parse comma-separated Hosts
        /// </summary>
        /// <param name="franchise">Franchise</param>
        /// <returns>Comma-separated hosts</returns>
        public static string[] ParseHostValues(this Franchise franchise)
        {
            if (franchise == null)
                throw new ArgumentNullException("franchise");

            var parsedValues = new List<string>();
            if (!String.IsNullOrEmpty(franchise.WebSite))
            {
                string[] hosts = franchise.WebSite.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string host in hosts)
                {
                    var tmp = host.Trim();
                    if (!String.IsNullOrEmpty(tmp))
                        parsedValues.Add(tmp);
                }
            }
            return parsedValues.ToArray();
        }

        /// <summary>
        /// Indicates whether a franchise contains a specified host
        /// </summary>
        /// <param name="franchise">Franchise</param>
        /// <param name="host">Host</param>
        /// <returns>true - contains, false - no</returns>
        public static bool ContainsHostValue(this Franchise franchise, string host)
        {
            if (franchise == null)
                throw new ArgumentNullException("franchise");

            if (String.IsNullOrEmpty(host))
                return false;

            var contains = franchise.ParseHostValues()
                                .FirstOrDefault(x => x.Equals(host, StringComparison.InvariantCultureIgnoreCase)) != null;
            return contains;
        }
    }
}
