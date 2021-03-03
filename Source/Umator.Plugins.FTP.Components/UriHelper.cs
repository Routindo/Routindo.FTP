using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umator.Plugins.FTP.Components
{
    public static class UriHelper
    {
        /// <summary>
        /// Combines the Uri with Segments 
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        /// <param name="path">The path.</param>
        /// <param name="filename">The filename.</param>
        /// <returns>The absolute URI</returns>
        public static string Combine(string host, int port, string path, string filename)
        {
            
            UriBuilder builder = new UriBuilder("ftp", host, port);
            return AppendToURL(builder.Uri.AbsoluteUri, path, filename);
        }

        /// <summary>
        /// Appends to URL.
        /// </summary>
        /// <param name="baseURL">The base URL.</param>
        /// <param name="segments">The segments.</param>
        /// <returns>The absolute Uri</returns>
        private static string AppendToURL(string baseURL, params string[] segments)
        {
            return string.Join("/", new[] {baseURL.TrimEnd('/')}
                .Concat(segments.Where(s=> !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim('/'))));
        }

        public static string BuildPath(params string[] segments)
        {
            return string.Join("/", segments.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim('/')));
        }
    }
}
