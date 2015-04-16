﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel.Syndication;
using System.Data;
using CDatos.DBEntities;
using CNegocio.Classes;

namespace CNegocio.Utils
{
    public class Utils
    {
        #region "Access DB service ItemDB"
        public static int saveItem(Item itemToSave, DateTime dtnow, int ido)
        {
            ItemDB itmdb = new ItemDB();
            itmdb.savedate = dtnow;
            itmdb.idoriginrss = ido;
            //itmdb.content = ;
            return ItemDB.saveItem(itmdb);
        }

        public static int updateItem(Item itemToSave, int id, DateTime dtnow, int ido)
        {
            return 0;
        }

        public static int deleteItem()
        {
            return 0;
        }

        public static List<ItemDB> retrieveItems()
        {
            List<ItemDB> listItems = new List<ItemDB>();

            return listItems;
        }
        #endregion

        #region "Access DB service RSSContactDB"
        public static int saveRssContact()
        {
            return 0;
        }

        public static int updateRssContact()
        {
            return 0;
        }

        public static int deleteRssContact()
        {
            return 0;
        }

        public static DataTable /*List<RSSContactDB>*/ retrieveRssContact()
        {
            //List<RSSContactDB> listRss = RSSContactDB.retrieveAllRss();

            return RSSContactDB.retrieveAllRss();
        }
        #endregion

        #region "Other methods"

        #endregion
    }
}