using System;
using System.Collections.Generic;
using System.Text;

namespace EfficientDelivery.CommonModels
{
    public class SearchItem
    {
        /// <summary>
        /// Id of search
        /// </summary>
        public virtual int Id { get; set; }

        /// <summary>
        /// Title of search to be displayed
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Description of search to be displayed
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Url of search page, where to get info from
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Defines if search is in active state for now. 
        /// </summary>
        public virtual bool IsActive { get; set; }
    }
}
