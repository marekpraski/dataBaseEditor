using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataBaseEditor;

namespace DataBaseEditorTests
{
    [TestClass]
    public class DBEditorMainTest
    {
        //żeby uruchomić poniższy test zmienić metodę getDBName na public i dodać jej parametr string sqlQuery
        //[TestMethod]
        //public void getDBNameTest()
        //{
        //    string sqlQuery = "select top 10 id_parc, nr_parc, pow_obrys, miazszosc,kat_upadu, siarka, bil  from [dbo].[2_parcele]";
        //    string dbName = "[dbo].[2_parcele]";
        //    DBEditorMainForm dbe = new DBEditorMainForm();
        //    Assert.AreEqual(dbName, dbe.getDBName(sqlQuery));
        //}

        [TestMethod]
        public void testTwo()
        {

        }
    }
}
