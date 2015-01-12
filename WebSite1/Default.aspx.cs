using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Abot.Crawler;
using Abot.Poco;
using System.Net;
using log4net.Config;
using HtmlAgilityPack;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        XmlConfigurator.Configure();

        //Will use app.config for confguration
        PoliteWebCrawler crawler = new PoliteWebCrawler();

        crawler.PageCrawlStartingAsync += crawler_ProcessPageCrawlStarting;
        crawler.PageCrawlCompletedAsync += crawler_ProcessPageCrawlCompleted;
        crawler.PageCrawlDisallowedAsync += crawler_PageCrawlDisallowed;
        crawler.PageLinksCrawlDisallowedAsync += crawler_PageLinksCrawlDisallowed;

        CrawlResult result = crawler.Crawl(new Uri("http://www.amazon.com/Beauty-Makeup-Skin-Hair-Products/b/ref=nav_shopall_bty?ie=UTF8&node=3760911"));

        if (result.ErrorOccurred)
            Console.WriteLine("Crawl of {0} completed with error: {1}", result.RootUri.AbsoluteUri, result.ErrorException.Message);
        else
            Console.WriteLine("Crawl of {0} completed without error.", result.RootUri.AbsoluteUri);

    }

    void crawler_ProcessPageCrawlStarting(object sender, PageCrawlStartingArgs e)
    {
        PageToCrawl pageToCrawl = e.PageToCrawl;
        Console.WriteLine("About to crawl link {0} which was found on page {1}", pageToCrawl.Uri.AbsoluteUri, pageToCrawl.ParentUri.AbsoluteUri);
    }

    void crawler_ProcessPageCrawlCompleted(object sender, PageCrawlCompletedArgs e)
    {
        CrawledPage crawledPage = e.CrawledPage;

        if (crawledPage.WebException != null || crawledPage.HttpWebResponse.StatusCode != HttpStatusCode.OK)
        {
            Console.WriteLine("Crawl of page failed {0}", crawledPage.Uri.AbsoluteUri);
        }
        else
        {
            Console.WriteLine("Crawl of page succeeded {0}", crawledPage.Uri.AbsoluteUri);
            NewMethod2(crawledPage);
        }

        if (string.IsNullOrEmpty(crawledPage.Content.Text))
            Console.WriteLine("Page had no content {0}", crawledPage.Uri.AbsoluteUri);
    }

    private void NewMethod2(CrawledPage crawledPage)
    {
        HtmlNode nextNode = crawledPage.HtmlDocument.GetElementbyId("productTitle");

        if (nextNode != null)
        {
            CrowledPages.Text = CrowledPages.Text + "<br>" + "PAGE URI: " + crawledPage.Uri.AbsoluteUri;
            CrowledPages.Text = CrowledPages.Text + "<br> Name: " + nextNode.InnerText;
            CrowledPages.Text = CrowledPages.Text + "<br> Price: " + crawledPage.HtmlDocument.GetElementbyId("priceblock_ourprice").InnerText;
            CrowledPages.Text = CrowledPages.Text + "<br>---------------------------------<br>";
        }

    }

    void crawler_PageLinksCrawlDisallowed(object sender, PageLinksCrawlDisallowedArgs e)
    {
        CrawledPage crawledPage = e.CrawledPage;
        Console.WriteLine("Did not crawl the links on page {0} due to {1}", crawledPage.Uri.AbsoluteUri, e.DisallowedReason);
    }

    void crawler_PageCrawlDisallowed(object sender, PageCrawlDisallowedArgs e)
    {
        PageToCrawl pageToCrawl = e.PageToCrawl;
        Console.WriteLine("Did not crawl page {0} due to {1}", pageToCrawl.Uri.AbsoluteUri, e.DisallowedReason);
    }
}
