using HtmlAgilityPack;
using System;
using System.Collections.Generic;

namespace TelegramBot
{
    public static class ParserManager
    {
        internal static List<CourseInfo> ParseBeetRootAcademy()
        {
            try
            {
                var url = "https://beetroot.academy/courses/online";
                var web = new HtmlWeb();
                HtmlDocument doc = web.Load(url);
                var info = doc.DocumentNode.SelectNodes("//a[(contains(@class, 'intro_box'))]");
                var courses = new List<CourseInfo>();
                foreach (var item in info)
                {
                    courses.Add(new CourseInfo($"{item.FirstChild.FirstChild.InnerText}", $"{item.FirstChild.LastChild.InnerText}", $"{url}{item.GetAttributeValue("href", string.Empty)}"));
                }
                Console.WriteLine("Parsing BRA complited!");
                return courses;
            }
            catch (Exception)
            {
                Console.WriteLine("Parsing BRA failed!");
                return new List<CourseInfo>();
            }
        }
        internal static List<CourseInfo> ParseMateAcademy()
        {
            try
            {
                var url = "https://mate.academy";
                var web = new HtmlWeb();
                HtmlDocument doc = web.Load(url);
                var info = doc.DocumentNode.SelectNodes("//section[@class='CourseCard_cardContainer__7_4lK']");
                List<CourseInfo> courses = new List<CourseInfo>();
                foreach (var item in info)
                {
                    courses.Add(new CourseInfo($"{item.FirstChild.InnerText}", $"{item.LastChild.FirstChild.InnerText}", $"{url}{item.FirstChild.GetAttributeValue("href", string.Empty)}"));
                }
                Console.WriteLine("Parsing MA complited!");
                return courses;
            }
            catch (Exception)
            {
                Console.WriteLine("Parsing MA failed!");
                return new List<CourseInfo>();
            }
        }
        internal static List<CourseInfo> ParseITea()
        {
            try
            {
                var url = "https://onlineitea.com.ua/courses-list/";
                var web = new HtmlWeb();
                HtmlDocument doc = web.Load(url);
                var info = doc.DocumentNode.SelectNodes("//div[@class='card__main']");
                List<CourseInfo> courses = new List<CourseInfo>();
                foreach (var item in info)
                {
                    courses.Add(new CourseInfo($"{item.FirstChild.InnerText}", $"{item.LastChild.InnerText}", $""));
                }
                info = doc.DocumentNode.SelectNodes("//a[@class='card']");
                int i = 0;
                foreach (var item in info)
                {
                    courses[i++].Link = item.GetAttributeValue("href", string.Empty);
                }
                Console.WriteLine("Parsing ITea complited!");
                return courses;
            }
            catch (Exception)
            {
                Console.WriteLine("Parsing ITea failed!");
                return new List<CourseInfo>();
            }
        }
        internal static List<CourseInfo> ParseGoIT()
        {
            try
            {
                var url = "https://goit.global/ua/courses/";
                var web = new HtmlWeb();
                HtmlDocument doc = web.Load(url);
                var info = doc.DocumentNode.SelectNodes("//div[@class='grid items-end h-full']");
                var courses = new List<CourseInfo>();
                foreach (var item in info)
                {
                    courses.Add(new CourseInfo($"{item.Element("h3").InnerText.Trim(' ', '\n')}", $"{item.Element("p").InnerText.Trim(' ', '\n')}", $"{item.Element("a").GetAttributeValue("href", string.Empty)}"));
                }
                Console.WriteLine("Parsing GoIT complited!");
                return courses;
            }
            catch (Exception)
            {
                Console.WriteLine("Parsing MA failed!");
                return new List<CourseInfo>();
            }
        }
    }
}