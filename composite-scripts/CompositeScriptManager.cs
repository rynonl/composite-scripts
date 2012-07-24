using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Text;

namespace CompositeScripts
{
    //[ParseChildren(false)]
    //[PersistChildren(true)]
    public class CompositeScriptManager : System.Web.UI.WebControls.WebControl
    {
        private List<ScriptReference> mlScripts = new List<ScriptReference>();
        private List<ScriptReference> mlStyleSheets = new List<ScriptReference>();
        private List<ServiceReference> mlServices = new List<ServiceReference>();

        private ScriptManager smCurrentScriptManager;

        #region "Properties"

        public List<ScriptReference> Scripts
        {
            get { return mlScripts; }
            set { mlScripts = value; }
        }

        public List<ServiceReference> Services
        {
            get { return mlServices; }
            set { mlServices = value; }
        }

        public List<ScriptReference> StyleSheets
        {
            get { return mlStyleSheets; }
            set { mlStyleSheets = value; }
        }
        #endregion

        #region "Page Events"

        protected override void CreateChildControls()
        {
            smCurrentScriptManager = new ScriptManager();
            Controls.Add(smCurrentScriptManager);

            base.CreateChildControls();
        }

        protected override void OnPreRender(System.EventArgs e)
        {
            LoadComposite();

            foreach (var scr in mlServices)
            {
                smCurrentScriptManager.Services.Add(scr);
            }

            base.OnPreRender(e);
        }

        #endregion

        #region "Helper Methods"

        private void LoadComposite()
        {
            foreach (ScriptReference sr in mlScripts)
            {
                smCurrentScriptManager.CompositeScript.Scripts.Add(RenderURL(sr));
            }

            if (mlStyleSheets.Count > 0)
            {
                string url = GetStyleSheetURL();

                LiteralControl lit = new LiteralControl();
                lit.Text = string.Format("<link type=\"text/css\" rel=\"stylesheet\" href={0}{1}{2} />", (char)34, url, (char)34);

                this.Page.Header.Controls.Add(lit);
            }
        }

        private string GetStyleSheetURL()
        {
            if (mlStyleSheets == null)
                return string.Empty;

            string ReturnValue = string.Empty;

            StringBuilder Builder = new StringBuilder();
            Builder.Append("/CompositeResourceHandler.ashx?t=text/css&f=");

            string[] StyleSheetPaths = (from Ref in mlStyleSheets select Ref.Path).ToArray();

            string URLPathSuffix = string.Join(",", StyleSheetPaths);
            Builder.Append(URLPathSuffix);

            ReturnValue = Builder.ToString();

            return ReturnValue;

        }           

        /// <summary>
        /// Manually replaces the ~/ in app paths with the actual app root.  
        /// This is so the people with non-standard development environments don't get errors, 
        /// as the ScriptManager will only handle duplicate scripts correctly if their paths are hardcoded.
        /// </summary>
        /// <param name="sr">A ScriptReference object</param>
        /// <returns> A ScriptReference with it's path hardcoded to the app root</returns>
        /// <remarks></remarks>
        private ScriptReference RenderURL(ScriptReference sr)
        {
            string strAppPath = this.Context.Request.ApplicationPath;
            if (!strAppPath.EndsWith("/"))
            {
                strAppPath = strAppPath + ("/");
            }
            sr.Path = sr.Path.Replace("~/", strAppPath);
            return sr;
        }

        #endregion
    }
}
