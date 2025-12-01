using cdn_api;
using GoNet_Comarch_SyncService.DTOs;
using GoNet_Comarch_SyncService.Services.Interfaces;
using GoNet_Comarch_SyncService.Settings;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Attribute = GoNet_Comarch_SyncService.DTOs.Attribute;

namespace GoNet_Comarch_SyncService.Services
{
    public class ErpApiClient : IErpApiClient
    {
        [DllImport("ClaRUN.dll")]
        public static extern void AttachThreadToClarion(int _flag);

        private readonly ErpApiSettings _settings;

        public ErpApiClient(IOptions<ErpApiSettings> options)
        {
            _settings = options.Value;
        }

        public int Login()
        {
            int sessionId = 0;
            XLLoginInfo_20241 xLLoginInfo = new()
            {
                Wersja = _settings.ApiVersion,
                ProgramID = _settings.ProgramName,
                Baza = _settings.Database,
                OpeIdent = _settings.Username,
                OpeHaslo = _settings.Password,
                TrybWsadowy = 1
            };

            int result = cdn_api.cdn_api.XLLogin(xLLoginInfo, ref sessionId);
            if (result != 0)
            {
                throw new Exception(CheckError(11, result));
            }
            return sessionId;
        }

        public int Logout(int sessionId)
        {
            AttachThreadToClarion(1);
            XLLogoutInfo_20241 xLLogoutInfo = new()
            {
                Wersja = _settings.ApiVersion,
            };

            int result = cdn_api.cdn_api.XLLogout(sessionId);
            if (result != 0)
            {
                throw new Exception(CheckError(11, result));
            }
            return result;
        }

        public int CreateClient(int sessionId, Client client)
        {
            AttachThreadToClarion(1);
            ManageTransaction(sessionId, 0); // Open transaction
            int erpClientId = 0;

            XLKontrahentInfo_20241 xlClient = new()
            {
                Wersja = _settings.ApiVersion,
                Akronim = client.Acronym,
                Nazwa1 = SubstringSafe(client.Name, 0, 51),
                Nazwa2 = SubstringSafe(client.Name, 51, 51),
                Nazwa3 = SubstringSafe(client.Name, 102, 251),
                NipE = client.NIP,
                Regon = client.Regon,
                Opis = client.Description,
                EMail = client.Email,
                Telefon1 = client.Phone,
                Miasto = client.Address.City,
                Ulica = client.Address.Street + " " + client.Address.Street,
                KodP = client.Address.PostalCode,
                Kraj = client.Address.Country
            };
            int createResult = cdn_api.cdn_api.XLNowyKontrahent(sessionId, ref erpClientId, xlClient);

            if (createResult != 0)
            {
                ManageTransaction(sessionId, 2); // Rollback transaction
                throw new Exception(CheckError(12, createResult));
            }

            XLZamkniecieKontrahentaInfo_20241 xlCloseClient = new()
            {
                Wersja = _settings.ApiVersion,
                GidNumer = erpClientId,
            };

            int closeResult = cdn_api.cdn_api.XLZamknijKontrahenta(sessionId, xlCloseClient);

            if (closeResult != 0)
            {
                ManageTransaction(sessionId, 2); // Rollback transaction
                throw new Exception(CheckError(13, closeResult));
            }

            try
            {
                foreach (var attribute in client.Attributes)
                {
                    AddAttribute(sessionId, erpClientId, 1, 0, attribute);
                }
            }
            catch (Exception ex)
            {
                ManageTransaction(sessionId, 2); // Rollback transaction
                throw new Exception(ex.Message);
            }

            ManageTransaction(sessionId, 1); // Commit transaction
            return erpClientId;
        }

        public int CreateClientBranch(int sessionId, ClientBranch branch)
        {
            AttachThreadToClarion(1);
            ManageTransaction(sessionId, 0); // Open transaction

            int erpBranchId = 0;
            XLAdresInfo_20241 xlBranch = new()
            {
                Wersja = _settings.ApiVersion,
                KntNumer = branch.BranchClientErpId,
                Akronim = branch.Acronym,
                Nazwa1 = SubstringSafe(branch.Name, 0, 51),
                Nazwa2 = SubstringSafe(branch.Name, 51, 51),
                Nazwa3 = SubstringSafe(branch.Name, 102, 251),
                NipE = branch.NIP,
                Regon = branch.Regon,
                EMail = branch.Email,
                Telefon1 = branch.Phone,
                Miasto = branch.Address.City,
                Ulica = branch.Address.Street + " " + branch.Address.Street,
                KodP = branch.Address.PostalCode,
                Kraj = branch.Address.Country,
                AdresBank = 1
            };
            int createResult = cdn_api.cdn_api.XLNowyAdres(sessionId, ref erpBranchId, xlBranch);

            if (createResult != 0)
            {
                ManageTransaction(sessionId, 2); // Rollback transaction
                throw new Exception(CheckError(14, createResult));
            }

            XLZamkniecieAdresuInfo_20241 xlCloseBranch = new()
            {
                Wersja = _settings.ApiVersion,
                GidNumer = erpBranchId,
            };

            int closeResult = cdn_api.cdn_api.XLZamknijAdres(sessionId, xlCloseBranch);

            if (closeResult != 0)
            {
                ManageTransaction(sessionId, 2); // Rollback transaction
                throw new Exception(CheckError(16, closeResult));
            }

            ManageTransaction(sessionId, 1); // Commit transaction
            return erpBranchId;
        }

        private int AddAttribute(int sessionId, int obiNumer, int obiType, int obiLp, Attribute attribute)
        {
            AttachThreadToClarion(1);
            XLAtrybutInfo_20241 xLAtrybut = new()
            {
                Wersja = _settings.ApiVersion,
                Klasa = attribute.ClassName,
                Wartosc = attribute.Value,
                GIDNumer = obiNumer,
                GIDTyp = obiType,
                GIDLp = obiLp,
                GIDSubLp = 0,
                GIDFirma = 449892,
            };

            int result = cdn_api.cdn_api.XLDodajAtrybut(sessionId, xLAtrybut);
            if (result != 0)
            {
                throw new Exception($"Nie udało się założyć atrybutu {attribute.ClassName} z wartością {attribute.Value}");
            }
            return result;
        }

        private string CheckError(int function, int errorCode)
        {
            XLKomunikatInfo_20241 xLKomunikat = new()
            {
                Wersja = _settings.ApiVersion,
                Funkcja = function,
                Blad = errorCode,
                Tryb = 0
            };
            int result = cdn_api.cdn_api.XLOpisBledu(xLKomunikat);

            if (result == 0)
                return xLKomunikat.OpisBledu;
            else
                return $"Error while checking error. Error code: {result}";
        }

        private int ManageTransaction(int sessionId, int type, string token = "")
        {
            XLTransakcjaInfo_20241 xLTransakcja = new()
            {
                Wersja = _settings.ApiVersion,
                Tryb = type
            };
            int result = cdn_api.cdn_api.XLTransakcja(sessionId, xLTransakcja);
            return result;
        }

        private string SubstringSafe(string input, int startIndex, int length)
        {
            if (string.IsNullOrEmpty(input) || startIndex >= input.Length)
                return string.Empty;

            return input.Substring(startIndex, Math.Min(length, input.Length - startIndex));
        }
    }
}