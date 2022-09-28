using System.Data.Common;
using System.Data.SQLite;
using System.Net;
using System.Xml;
namespace Lab_7_8
{
    public partial class Form1 : Form
    {
        string strNews;
        
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://news.yandex.ru/auto.rss");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);
            strNews = reader.ReadToEnd();
            richTextBox1.Text = strNews;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            SQLiteConnection db;
            db = new SQLiteConnection(@"Data Source = D:\Программирование\Lab 7-8\dataBase.db;");
            db.Open();
            SQLiteCommand command;
            command = new SQLiteCommand("DELETE FROM News", db);
            command.ExecuteNonQuery();
            command = new SQLiteCommand(@"PRAGMA synchronous = 1;
                            CREATE TABLE IF NOT EXISTS News(Id INTEGER PRIMARY KEY
                            AUTOINCREMENT, Title, Link, Description, PubDate); ", db);
            command.ExecuteNonQuery();
            XmlDocument xmlNews = new XmlDocument();
            xmlNews.Load("https://news.yandex.ru/auto.rss");
            XmlNodeList childNodeList =
            xmlNews.DocumentElement.SelectSingleNode("channel").SelectNodes("item");
            foreach (XmlNode xmlNode in childNodeList)
            {
                string title = xmlNode.SelectSingleNode("title").InnerText;
                string link = xmlNode.SelectSingleNode("link").InnerText;
                string description = xmlNode.SelectSingleNode("description").InnerText;
                string pubDate = xmlNode.SelectSingleNode("pubDate").InnerText;
                command = new SQLiteCommand("INSERT INTO News (Title, Link, Description, PubDate) VALUES (@title, @link, @description, @pubDate)", db);
                command.Parameters.AddWithValue("@title", $"{title}");
                command.Parameters.AddWithValue("@link", $"{link}");
                command.Parameters.AddWithValue("@description", $"{description}");
                command.Parameters.AddWithValue("@pubDate", $"{pubDate}");
                command.ExecuteNonQuery();
            }

            SQLiteDataReader reader;
            command = new SQLiteCommand("SELECT * FROM News", db);
            reader = command.ExecuteReader();
            int k = 0;
            foreach (DbDataRecord item in reader)
            {
                k++;
                string title = item["Title"].ToString();
                string pubDate = item["PubDate"].ToString();
                string link = item["Link"].ToString();
                string description = item["Description"].ToString();
                dataGridView1.Rows.Add(k, title, link, description, pubDate);
            }







        }

        
    }
}