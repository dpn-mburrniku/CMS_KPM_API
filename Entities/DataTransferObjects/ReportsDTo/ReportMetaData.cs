using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects.ReportsDTo
{
    public class ReportMetaData
    {
    }

    public class PaymentReportMetaData
    {
        public string ReportTitle { get; set; } = "Raporti i pagesave";
        public string DateFrom { get; set; }
        public string DateTo { get; set; }

    }

    public class PaymentReportVM
    {
        public string Data { get; set; }
        public decimal? Shuma { get; set; }
        public string Sukses { get; set; }
        public DateTime Date { get; set; }
    }

    public class TreeTypesForLocationReportMetaData
    {
        public string ReportTitle { get; set; } = "Raporti i mbjelljeve per Lokacione";

    }

    public class TreeTypesForLocationReportReportVM
    {
        public int Id { get; set; }
        public int MunicipalityId { get; set; }
        public string Municipality { get; set; }
        public int MunicipalityLocationId { get; set; }
        public string strMunicipalityLocation { get; set; }
        public int TreeTypeId { get; set; }
        public string strTreeType { get; set;}
        public decimal? TotalForPlanting { get; set; }
        public decimal? TotalPlanted { get; set; }
        public decimal? Price { get; set; }
    }

    public class CertificatesForLocationReportMetaData
    {
        public string ReportTitle { get; set; } = "Raporti i Çertifikatave për Lokacione";
        public string DateFrom { get; set; }
        public string DateTo { get; set; }  

    }

    public class CertificatesForLocationReportReportVM
    {
        public string Reference { get; set; }
        public int MunicipalityId { get; set; }
        public string strMunicipality { get; set; }
        public int LocationId { get; set; }
        public string strLocation { get; set; }
        public int TreeTypeId { get; set; }
        public string strTreeType { get; set; }
        public int? TypeId { get; set; }
        public string strType { get; set; }
        public string QRCodeLink { get; set; }
        public int? PaymentId { get; set; }

        public decimal? Amount { get; set; }
        public decimal? Price { get; set; }
        public decimal? TotalPrice { get; set; }
        public string Created { get; set; }
    }
}
