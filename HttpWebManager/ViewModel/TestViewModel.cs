using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Net;
using XpoObjects;
using DevExpress.Xpo;
using Telfort_XPO_Objects;
using DevExpress.Xpo.DB;
using System.Configuration;
using System.Web;
using System.Threading;
using System.Reflection;

namespace HttpWebManager
{
    public class TestViewModel : ScrapingBase
    {
        private void Execute(Object sender, EventArgs e)
        {
            using (UnitOfWork session1 = Using<UnitOfWork>())
            {
                var _Lookup_AboBundelList = new XPQuery<Lookup_AboBundel>(session1).ToList().Where(x => x.Actief == true).ToList();
                var xx = _Lookup_AboBundelList;
                string vv = "vv";
                int s = 3 + 2;
                vv = s.ToString();
            }
            //Domain.InitTypeContract();
            //var zz = ContractViewModelLookup();
            //ContractViewModelLookup().First().Run();
            //getAssem();
            //GetViewModel((int)Telfort_Objects.ContractType.Nieuw);
        }

        public override void Run()
        {
            //
        }

        private void GetViewModel(Telfort_Objects.ContractType _ContractType)
        {
            ScrapingBase[] ViewModel = new ScrapingBase[3];

            ViewModel[(int)Telfort_Objects.ContractType.Nieuw] = Using<NuAanmeldenViewModel>();
            ViewModel[(int)Telfort_Objects.ContractType.Pre2Post] = Using<NuPre2PostViewModel>();
            ViewModel[(int)Telfort_Objects.ContractType.Verlengen] = Using<NuVerlengenViewModel>();

            ViewModel[(int)_ContractType].Run();
        }

        private static ScrapingBase[] ContractViewModelLookup()
        {
            ScrapingBase[] ViewModel = new ScrapingBase[3];
            ViewModel.GetType().Assembly.GetTypes().ToList()
                .Where(x => x.IsSubclassOf(typeof(ScrapingBase))).ToList()
                .ForEach(y => 
            {                
                object _NewObject = y.GetCustomAttributes(true).Where(x => x.GetType().Name.Equals("LookupViewModel")).FirstOrDefault();
                if (_NewObject != null)
                {
                    object _ContractType = _NewObject.GetType().GetProperty("ContractType").GetValue(_NewObject, null).ToString();
                    if (_ContractType != null)
                    {
                        //ScrapingBase abcde = (ScrapingBase)y.GetConstructor(Type.EmptyTypes).Invoke(null);
                        ViewModel[_ContractType.ToInt16()] = (ScrapingBase)AssemblyManager.CreateObjectInstance(y.Name);
                    }
                }
            });
            return ViewModel;
        }
                
        private void Test1()
        {
            InitControlsBinding();
            InvokeBezig();

            string strResponseData = this.Result;

            string SelectHTMLString = HttpWebManager.ScrapeHelper.ExtractValue(strResponseData, "<select name=\"gender\" id=\"gender\" style=\"width: 100% \">", "</select>");
            Domain.UpdateComboBoxValues<Lookup_Geslacht>(SelectHTMLString);

        }


    }
}
