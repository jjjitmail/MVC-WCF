using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using System.Configuration;
using Telfort_XPO_Objects;
using System.Configuration;
using DevExpress.Xpo.Metadata;
using System.Reflection;
using System.Globalization;

namespace HttpWebManager
{
    public class LookupBundelViewModel
    {
        public static Telfort_Objects.TelfortBundels Execute()
        {
            Domain.InitDBConnection();
            using (UnitOfWork session1 = new UnitOfWork())
            {
                return GetBundel(session1);
            }
        }

        private static Telfort_Objects.TelfortBundels GetBundel(UnitOfWork session1)
        {
            var _Lookup_AboBundelList = new XPQuery<Lookup_AboBundel>(session1).ToList().Where(x=> x.Actief == true).ToList();
            //var AboType = new XPQuery<Lookup_TypeProduct>(session1).ToList().Where(x=> x.Value == "0").ToList();

            Telfort_Objects.TelfortBundels _TelfortBundels = new Telfort_Objects.TelfortBundels();
            var _MaandenList = _Lookup_AboBundelList.ToList().Select(x => x.AantalMaanden).Distinct();

            if (_Lookup_AboBundelList != null)
            {
                Telfort_Objects.Bundel.BundelType _BundelType = Telfort_Objects.Bundel.BundelType.Nieuw;
                List<Telfort_Objects.Bundel> _BundelList = new List<Telfort_Objects.Bundel>();

                XPCollection<Lookup_TypeContract> _Lookup_TypeContractList = new XPCollection<Lookup_TypeContract>(session1);

                //Enum.GetNames(_BundelType.GetType()).ToList().ForEach(x =>
                _Lookup_TypeContractList.ToList().OrderBy(a=> a.Value).ToList().ForEach(x =>
                {
                    Telfort_Objects.Bundel _Bundel = new Telfort_Objects.Bundel() { Name = x.Name, Value = x.Value };
                    //List<Telfort_Objects.BundelAansluiting> _BundelAansluitingList = new List<Telfort_Objects.BundelAansluiting>();

                    // aantal aansluiting type doorlopen
                    //AboType.ToList().ForEach(y =>
                    //{
                        //Telfort_Objects.BundelAansluiting _BundelAansluiting = new Telfort_Objects.BundelAansluiting() { Name = y.Name, Value = y.Value };
                        List<Telfort_Objects.BundelAbonnement> _BundelAbonnementList = new List<Telfort_Objects.BundelAbonnement>();

                        // aantal maanden doorlopen
                        _MaandenList.ToList().ForEach(AantalMaanden =>
                        {
                            // aantal abonnement type doorlopen
                            _Lookup_AboBundelList.AsParallel()
                                 .Where(z => z.KeyCombo.ToLower().StartsWith(x.Name.ToLower()) && z.ProductType == 0 && z.AantalMaanden == AantalMaanden).ToList()
                                 .Select(z1 => z1.AboType).Distinct().ToList().ForEach(z2 =>
                            {
                                string AboTypeValue = _Lookup_AboBundelList
                                 .Where(z => z.KeyCombo.ToLower().StartsWith(x.Name.ToLower()) && z.ProductType == 0 && z.AantalMaanden == AantalMaanden && z.AboType == z2).First().AboTypeValue;

                                Telfort_Objects.BundelAbonnement _BundelAbonnement = new Telfort_Objects.BundelAbonnement() { AantalMaanden = AantalMaanden, Name = z2, Value = AboTypeValue };
                                List<Telfort_Objects.BundelProductGroup> _BundelProductGroupList = new List<Telfort_Objects.BundelProductGroup>();

                                // aantal product group doorlopen
                                _Lookup_AboBundelList.AsParallel()
                                    .Where(q => q.KeyCombo.ToLower().StartsWith(x.Name.ToLower()) && q.ProductType == 0
                                        && q.AantalMaanden == AantalMaanden && q.AboType == z2).ToList()
                                        .Select(q1 => q1.ProductGroupName).Distinct().ToList().ForEach(q2 =>
                                {
                                    Telfort_Objects.BundelProductGroup _BundelProductGroup = new Telfort_Objects.BundelProductGroup() { Name = q2 };
                                    List<Telfort_Objects.BundelProduct> _BundelProductList = new List<Telfort_Objects.BundelProduct>();

                                    // aantal producten doorlopen
                                    _Lookup_AboBundelList.AsParallel()
                                        .Where(s => s.KeyCombo.ToLower().StartsWith(x.Name.ToLower()) && s.ProductType == 0
                                            && s.AantalMaanden == AantalMaanden && s.AboType == z2 && s.ProductGroupName == q2).ToList()
                                            .ForEach(s1 =>
                                    {
                                        Telfort_Objects.BundelProduct _BundelProduct = new Telfort_Objects.BundelProduct();
                                        AssemblyManager.ConvertObject(_BundelProduct, s1);
                                        _BundelProductList.Add(_BundelProduct);
                                    });
                                    _BundelProductGroup.BundelProductList = _BundelProductList;
                                    _BundelProductGroupList.Add(_BundelProductGroup);
                                });
                                _BundelAbonnement.BundelProductGroupList = _BundelProductGroupList;
                                _BundelAbonnementList.Add(_BundelAbonnement);
                            });
                        });
                        //_BundelAansluiting.BundelAbonnementList = _BundelAbonnementList;
                        //_BundelAansluitingList.Add(_BundelAansluiting);
                        _Bundel.BundelAbonnementList = _BundelAbonnementList;
                    //});
                    //_Bundel.BundelAansluitingList = _BundelAansluitingList;                    
                    _BundelList.Add(_Bundel);
                });
                _TelfortBundels.BundelList = _BundelList;
            }
            return _TelfortBundels;
        }

    }
}
