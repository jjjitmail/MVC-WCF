using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telfort_XPO_Objects;
using DevExpress.Xpo;

namespace HttpWebManager
{
    public class TelfortKlantViewModel : ScrapingBase
    {
        private void Execute(Object sender, EventArgs e)
        {
            AAS_TelfortService.AasTelfortClient _AasTelfortClient = new AAS_TelfortService.AasTelfortClient();

            //Telfort_Objects.Contract _Contract = _AasTelfortClient.GetContract();

            //var ss = _Contract.Klant.Achternaam;

            //AAS_TelfortService.HttpWebResult _HttpWebResult = _AasTelfortClient.Set(null);
        }

        public override void Run()
        {
            //
        }

        public static Telfort_Objects.Klant Get(string MobielNr, string SimOfRekeningNr)
        {
            Domain.InitDBConnection();
            Telfort_Objects.Klant _Klant = new Telfort_Objects.Klant();

            using (UnitOfWork session1 = new UnitOfWork())
            {
                var _XPOKlant = new XPQuery<Klant>(session1).ToList()
                    .Where(z=> z.ContractCollection
                        .Where(x => x.AbonnementCollection
                            .Where(y => y.MobileNr == MobielNr && (x.Rekeningnummer == SimOfRekeningNr || y.SimNr == SimOfRekeningNr))
                            .Count() > 0)
                            .Count() > 0)
                            .FirstOrDefault();;
                
                if (_XPOKlant != null)
                {               
                    AssemblyManager.ConvertObject(_Klant, _XPOKlant);

                    Telfort_Objects.Adres _Adres = new Telfort_Objects.Adres();
                    AssemblyManager.ConvertObject(_Adres, _XPOKlant.AdresCollection.ToList().OrderByDescending(x=> x.DateCreated).First());
                    _Klant.AdresList = new List<Telfort_Objects.Adres>();
                    _Klant.AdresList.Add(_Adres);

                    _Klant.AbonnementContractList = new List<Telfort_Objects.AbonnementContract>();
                    _XPOKlant.ContractCollection.ToList().ForEach(x => 
                    {
                        Telfort_Objects.AbonnementContract _AbonnementContract = new Telfort_Objects.AbonnementContract();
                        AssemblyManager.ConvertObject(_AbonnementContract, x);

                        _AbonnementContract.AbonnementList = new List<Telfort_Objects.Abonnement>();
                        x.AbonnementCollection.ToList().ForEach(y => 
                        {
                            Telfort_Objects.Abonnement _Abonnement = new Telfort_Objects.Abonnement();
                            AssemblyManager.ConvertObject(_Abonnement, y);

                            _Abonnement.AbonnementProductList = new List<Telfort_Objects.AbonnementProduct>();
                            y.AbonnementProductCollection.ToList().ForEach(z => 
                            {
                                Telfort_Objects.AbonnementProduct _AbonnementProduct = new Telfort_Objects.AbonnementProduct();
                                AssemblyManager.ConvertObject(_AbonnementProduct, z);
                                _Abonnement.AbonnementProductList.Add(_AbonnementProduct);
                            });
                            _AbonnementContract.AbonnementList.Add(_Abonnement);                            
                        });
                        _Klant.AbonnementContractList.Add(_AbonnementContract);
                    });
                }
            }            
            return _Klant;
        }

        private static void ToXPO(Telfort_Objects.Contract _Contract)
        {
            //return null;
        }

        public static HttpWebResult Set(Telfort_Objects.Contract _Contract)
        {
            HttpWebResult _HttpWebResult = new HttpWebResult();

            _HttpWebResult = ContractCheck(_Contract);

            if (!_HttpWebResult.IsSuccess)
                return _HttpWebResult;

            Domain.InitDBConnection();

            try
            {
                using (UnitOfWork session1 = new UnitOfWork())
                {
                    Contract _XPO_Contract = new Contract(session1);

                    Klant _XPO_Klant = new Klant(session1);
                    AssemblyManager.ConvertObject(_XPO_Klant, _Contract.Klant);

                    _Contract.Klant.AdresList.ForEach(x =>
                    {
                        Adres _Adres = Using<Adres>(session1);
                        AssemblyManager.ConvertObject(_Adres, x);
                        _XPO_Klant.AdresCollection.Add(_Adres);
                    });

                    _Contract.Klant.AbonnementContractList.ForEach(x =>
                    {
                        AbonnementContract _AbonnementContract = Using<AbonnementContract>(session1);
                        AssemblyManager.ConvertObject(_AbonnementContract, x);

                        x.AbonnementList.ForEach(y =>
                        {
                            Abonnement _Abonnement = Using<Abonnement>(session1);
                            AssemblyManager.ConvertObject(_Abonnement, y);

                            y.AbonnementProductList.ForEach(z =>
                            {
                                AbonnementProduct _AbonnementProduct = Using<AbonnementProduct>(session1);
                                AssemblyManager.ConvertObject(_AbonnementProduct, z);
                                _Abonnement.AbonnementProductCollection.Add(_AbonnementProduct);
                            });

                            _AbonnementContract.AbonnementCollection.Add(_Abonnement);
                        });

                        _XPO_Klant.ContractCollection.Add(_AbonnementContract);
                    });
                    _XPO_Contract.Klant = _XPO_Klant;
                    session1.CommitChanges();
                    _HttpWebResult.IsSuccess = true;
                }                
            }
            catch (Exception err)
            {
                _HttpWebResult.IsSuccess = false;
                _HttpWebResult.ErrorMessage = err.Message;
            }
            return _HttpWebResult;
        }

        public static HttpWebResult ContractCheck(Telfort_Objects.Contract _Contract)
        {
            HttpWebResult _HttpWebResult = new HttpWebResult();
            _HttpWebResult.IsSuccess = true;
            return _HttpWebResult;
        }

        public static HttpWebResult ContractNaarTelfortSturen(Telfort_Objects.Contract _Contract)
        {
            HttpWebResult _HttpWebResult = new HttpWebResult();

            try
            {
                // zoek naar juist viewModel voor "contract type"
                ScrapingBase _ScrapingBase = ContractViewModelLookup(_Contract.Klant.AbonnementContractList.First().Type_Contract);

                // contract vullen
                _ScrapingBase.DealerContract = _Contract;

                // daarna contract naar telfort inschieten
                _ScrapingBase.Run();

                // result ophalen
                _HttpWebResult = _ScrapingBase.HttpWebResult;
            }
            catch (Exception err)
            {
                _HttpWebResult.IsSuccess = false;
                _HttpWebResult.ErrorMessage = err.Message;
            }
            
            return _HttpWebResult;
        }

        /// <summary>
        /// Alle nodige ViewModels worden automatisch in aan array gezet, 
        /// op deze manier kan de event opgeroepen worden aan de hand van de index/value vanuit andere functie.
        /// zo wordt de "if & else-jes" niet meer nodig
        /// </summary>
        /// <param name="_Type">Contract type (index)</param>
        /// <returns>juiste ViewModel</returns>
        private static ScrapingBase ContractViewModelLookup(int _Type)
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
                            ViewModel[_ContractType.ToInt16()] = (ScrapingBase)AssemblyManager.CreateObjectInstance(y.Name);
                    }//ScrapingBase zz = (ScrapingBase)y.GetConstructor(Type.EmptyTypes).Invoke(null);
                });
            return ViewModel[_Type];
        }
    }
}