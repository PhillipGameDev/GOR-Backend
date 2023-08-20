using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace GameOfRevenge.WebAdmin
{
    public static class Helper
    {
        public static bool IsRegion(ViewDataDictionary viewData, string name)
        {
            var resp = false;
            if ((viewData != null) && viewData.ContainsKey("Region"))
            {
                resp = (viewData["Region"] as string) == name;
            }

            return resp;
        }

        public static ViewDataDictionary ViewRegion(ViewDataDictionary viewData, string name)
        {
            var dic = new ViewDataDictionary(viewData);
            if (dic.ContainsKey("Region"))
            {
                dic["Region"] = name;
            }
            else
            {
                dic.Add("Region", name);
            }

            return dic;
        }
    }
}
