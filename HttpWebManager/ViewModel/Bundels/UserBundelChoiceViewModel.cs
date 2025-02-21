using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Xpo;
using Telfort_XPO_Objects;
using System.Linq.Expressions;

namespace HttpWebManager
{
    public class UserBundelChoiceViewModel : ScrapingBase
    {
        public UserBundelChoiceViewModel() : base() {}

        public UserBundelChoiceViewModel(Telfort_Objects.Contract _InputContract) : base() { DealerContract = _InputContract; UserContract = _InputContract; }

        private static Telfort_Objects.Contract UserContract;

        private static Telfort_Objects.UserBundelChoice CreateBundel_Aanmelden()
        {
            Telfort_Objects.UserBundelChoice _UserBundelChoice = new Telfort_Objects.UserBundelChoice();

            using (UnitOfWork session1 = new UnitOfWork())
            {
                var _Lookup_AboBundelList = new XPQuery<Lookup_AboBundel>(session1).ToList().Where(x => x.Actief == true).ToList();
                UserContract.Klant.AbonnementContractList.First().AbonnementList.First().AbonnementProductList
                .ForEach(x =>
                {
                    string aboId = x.BundelId;
                    var Lookup_AboBundel = _Lookup_AboBundelList.Where(y => y.BundelId == x.BundelId).FirstOrDefault();
                    if (Lookup_AboBundel.ValueId == "0001.0102")
                    {
                        _UserBundelChoice.Privilege = String.Format(",,,{0},{1};", Lookup_AboBundel.GroupName, Lookup_AboBundel.ValueId);
                        _UserBundelChoice.Privilege += String.Format(",,,{0},{1};", Lookup_AboBundel.ValueId, Lookup_AboBundel.Value);
                        _UserBundelChoice.GekozenSimOnly = Lookup_AboBundel.Value;
                    }
                    else
                    {
                        if (Lookup_AboBundel.ValueId == "0001.0103")
                        {
                            _UserBundelChoice.GekozenSmsBundel = Lookup_AboBundel.ValueId;
                            _UserBundelChoice.GekozenSimOnly_GekozenSmsBundel = string.Format("@{0}@{1}@", _UserBundelChoice.GekozenSimOnly, Lookup_AboBundel.Value);
                            _UserBundelChoice.GekozenSmsBundelOptie = Lookup_AboBundel.Value;
                        }
                        if (Lookup_AboBundel.ValueId == "0001.0104")
                        {
                            _UserBundelChoice.Surf_Mail = Lookup_AboBundel.ValueId;
                            _UserBundelChoice.Surf_Mail_Optie = Lookup_AboBundel.Value;
                            _UserBundelChoice.Surf_Mail_SimOnly = string.Format("@{0}@", _UserBundelChoice.GekozenSimOnly);
                            _UserBundelChoice.Surf_Mail_SimOnly += !string.IsNullOrEmpty(_UserBundelChoice.GekozenSmsBundel) ? _UserBundelChoice.GekozenSmsBundelOptie + "@" : Lookup_AboBundel.Value + "@";
                        }

                        _UserBundelChoice.AppendProduct += String.Format(",,,{0},{1};", Lookup_AboBundel.GroupName, Lookup_AboBundel.ValueId);
                        _UserBundelChoice.AppendProduct += String.Format(",,,{0},{1};", Lookup_AboBundel.ValueId, Lookup_AboBundel.Value);
                    }
                });
                _UserBundelChoice.Ids = String.Format(",,,0001,0001.0101;{0}{1}", _UserBundelChoice.Privilege, _UserBundelChoice.AppendProduct);
                
            }
            return _UserBundelChoice;
        }

        private static Telfort_Objects.UserBundelChoice CreateBundel_Verlengen()
        {
            Telfort_Objects.UserBundelChoice _UserBundelChoice = new Telfort_Objects.UserBundelChoice();

            using (UnitOfWork session1 = new UnitOfWork())
            {
                var _Lookup_AboBundelList = new XPQuery<Lookup_AboBundel>(session1).ToList().Where(x => x.Actief == true).ToList();
                UserContract.Klant.AbonnementContractList.First().AbonnementList.First().AbonnementProductList
                .ForEach(x =>
                {
                    string aboId = x.BundelId;
                    var Lookup_AboBundel = _Lookup_AboBundelList.Where(y => y.BundelId == x.BundelId).FirstOrDefault();
                    if (Lookup_AboBundel.ValueId == "0001.0101")
                    {
                        _UserBundelChoice.Privilege = String.Format(",,,{0},{1};", Lookup_AboBundel.GroupName, Lookup_AboBundel.ValueId);
                        _UserBundelChoice.Privilege += String.Format(",,,{0},{1};", Lookup_AboBundel.ValueId, Lookup_AboBundel.Value);
                        _UserBundelChoice.GekozenSimOnly = Lookup_AboBundel.Value;
                    }
                    else
                    {
                        if (Lookup_AboBundel.ValueId == "0001.0102")
                        {
                            _UserBundelChoice.GekozenSmsBundel = Lookup_AboBundel.ValueId;
                            _UserBundelChoice.GekozenSimOnly_GekozenSmsBundel = string.Format("@{0}@{1}@", _UserBundelChoice.GekozenSimOnly, Lookup_AboBundel.Value);
                            _UserBundelChoice.GekozenSmsBundelOptie = Lookup_AboBundel.Value;
                        }
                        if (Lookup_AboBundel.ValueId == "0001.0103")
                        {
                            _UserBundelChoice.Surf_Mail = Lookup_AboBundel.ValueId;
                            _UserBundelChoice.Surf_Mail_Optie = Lookup_AboBundel.Value;
                            _UserBundelChoice.Surf_Mail_SimOnly = string.Format("@{0}@", _UserBundelChoice.GekozenSimOnly);
                            _UserBundelChoice.Surf_Mail_SimOnly += !string.IsNullOrEmpty(_UserBundelChoice.GekozenSmsBundel) ? _UserBundelChoice.GekozenSmsBundelOptie + "@" : Lookup_AboBundel.Value + "@";
                        }

                        _UserBundelChoice.AppendProduct += String.Format(",,,{0},{1};", Lookup_AboBundel.GroupName, Lookup_AboBundel.ValueId);
                        _UserBundelChoice.AppendProduct += String.Format(",,,{0},{1};", Lookup_AboBundel.ValueId, Lookup_AboBundel.Value);
                    }
                });
                _UserBundelChoice.Ids = _UserBundelChoice.Privilege + _UserBundelChoice.AppendProduct;
            }
            return _UserBundelChoice;
        }

        private static Telfort_Objects.UserBundelChoice CreateBundel_Pre2Post()
        {
            Telfort_Objects.UserBundelChoice _UserBundelChoice = new Telfort_Objects.UserBundelChoice();

            using (UnitOfWork session1 = new UnitOfWork())
            {
                var _Lookup_AboBundelList = new XPQuery<Lookup_AboBundel>(session1).ToList().Where(x => x.Actief == true).ToList();
                UserContract.Klant.AbonnementContractList.First().AbonnementList.First().AbonnementProductList
                .ForEach(x =>
                {
                    string aboId = x.BundelId;
                    var Lookup_AboBundel = _Lookup_AboBundelList.Where(y => y.BundelId == x.BundelId).FirstOrDefault();
                    if (Lookup_AboBundel.ValueId == "0001.0101")
                    {
                        _UserBundelChoice.Privilege = String.Format(",,,{0},{1};", Lookup_AboBundel.GroupName, Lookup_AboBundel.ValueId);
                        _UserBundelChoice.Privilege += String.Format(",,,{0},{1};", Lookup_AboBundel.ValueId, Lookup_AboBundel.Value);
                        _UserBundelChoice.GekozenSimOnly = Lookup_AboBundel.Value;
                    }
                    else
                    {
                        if (Lookup_AboBundel.ValueId == "0001.0102")
                        {
                            _UserBundelChoice.GekozenSmsBundel = Lookup_AboBundel.ValueId;
                            _UserBundelChoice.GekozenSimOnly_GekozenSmsBundel = string.Format("@{0}@{1}@", _UserBundelChoice.GekozenSimOnly, Lookup_AboBundel.Value);
                            _UserBundelChoice.GekozenSmsBundelOptie = Lookup_AboBundel.Value;
                        }
                        if (Lookup_AboBundel.ValueId == "0001.0103")
                        {
                            _UserBundelChoice.Surf_Mail = Lookup_AboBundel.ValueId;
                            _UserBundelChoice.Surf_Mail_Optie = Lookup_AboBundel.Value;
                            _UserBundelChoice.Surf_Mail_SimOnly = string.Format("@{0}@", _UserBundelChoice.GekozenSimOnly);
                            _UserBundelChoice.Surf_Mail_SimOnly += !string.IsNullOrEmpty(_UserBundelChoice.GekozenSmsBundel) ? _UserBundelChoice.GekozenSmsBundelOptie + "@" : Lookup_AboBundel.Value + "@";
                        }

                        _UserBundelChoice.AppendProduct += String.Format(",,,{0},{1};", Lookup_AboBundel.GroupName, Lookup_AboBundel.ValueId);
                        _UserBundelChoice.AppendProduct += String.Format(",,,{0},{1};", Lookup_AboBundel.ValueId, Lookup_AboBundel.Value);
                    }
                });
                _UserBundelChoice.Ids = _UserBundelChoice.Privilege + _UserBundelChoice.AppendProduct;
            }
            return _UserBundelChoice;
        }

        public override void Run() { }

        public static Telfort_Objects.UserBundelChoice GetUserBundelChoiceCollection(Telfort_Objects.Contract _Contract)
        {
            Telfort_Objects.UserBundelChoice[] _UserBundelChoice = new Telfort_Objects.UserBundelChoice[3];

            _UserBundelChoice[(int)Telfort_Objects.ContractType.Nieuw] = CreateBundel_Aanmelden();
            _UserBundelChoice[(int)Telfort_Objects.ContractType.Verlengen] = CreateBundel_Verlengen();
            _UserBundelChoice[(int)Telfort_Objects.ContractType.Pre2Post] = CreateBundel_Pre2Post();
            
            return UserBundelChoice = _UserBundelChoice[_Contract.Klant.AbonnementContractList.First().Type_Contract];
        }

    }
}
