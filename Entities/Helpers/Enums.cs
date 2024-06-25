using Entities.DataTransferObjects;
using System.ComponentModel.DataAnnotations;
using System.Security.AccessControl;
using System.Xml.Linq;

namespace CMS.API.Helpers
{
    public enum ErrorStatus
    {
        Success = 1,
        Info = 2,
        Warning = 3,
        Error = 4,
        Secondary = 5,
        Primary = 6
    }

    public enum LanguageEnum
    {
        Albania = 1,
        English = 2,       
        Serbian = 3
    }

    public enum GenderEnum
    {
        Mashkull = 1,
        Femer = 2,        
    }

    public enum SysMenuType
    {
        Tittle = 1,
        Collapse = 2,
        Item = 3
    }

    public enum ActionType
    {
        LogOut = 1,
        SignIn = 2,
        IncorrectPassword = 3,
        NotAllowed = 4
    }

    public enum SequenceType
    {
        Page = 0,
        Menu = 1,
        Post = 2,
        Certificate = 100
    }

    public enum ReportType
    {
        PDF = 1,
        Excel = 2,
        Word = 3        
    }        
}
