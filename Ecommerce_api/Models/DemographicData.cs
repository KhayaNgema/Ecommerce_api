using System.ComponentModel.DataAnnotations;

namespace Ecommerce_api.Models
{
    public enum Gender
    {
        [Display(Name = "Male")]
        Male,

        [Display(Name = "Female")]
        Female
    }

    public enum Title
    {
        [Display(Name = "Mr.")]
        Mr,

        [Display(Name = "Mrs.")]
        Mrs,

        [Display(Name = "Miss")]
        Miss,

        [Display(Name = "Ms.")]
        Ms,

        [Display(Name = "Dr.")]
        Dr,

        [Display(Name = "Prof.")]
        Prof
    }

    public enum Ethnicity
    {
        [Display(Name = "Zulu")]
        Zulu,

        [Display(Name = "Xhosa")]
        Xhosa,

        [Display(Name = "Afrikaner")]
        Afrikaner,

        [Display(Name = "Sotho")]
        Sotho,

        [Display(Name = "Tswana")]
        Tswana,

        [Display(Name = "Pedi")]
        Pedi,

        [Display(Name = "Venda")]
        Venda,

        [Display(Name = "Tsonga")]
        Tsonga,

        [Display(Name = "Swazi")]
        Swazi,

        [Display(Name = "Ndebele")]
        Ndebele,

        [Display(Name = "Cape Coloured")]
        CapeColoured,

        [Display(Name = "Indian South African")]
        IndianSouthAfrican
    }

    public enum HomeLanguage
    {
        [Display(Name = "English")]
        English,

        [Display(Name = "isiZulu")]
        isiZulu,

        [Display(Name = "isiXhosa")]
        isiXhosa,

        [Display(Name = "Afrikaans")]
        Afrikaans,

        [Display(Name = "isiNdebele")]
        isiNdebele,

        [Display(Name = "Sesotho sa Leboa")]
        Sesotho_sa_Leboa,

        [Display(Name = "Sesotho")]
        Sesotho,

        [Display(Name = "Setswana")]
        Setswana,

        [Display(Name = "siSwati")]
        siSwati,

        [Display(Name = "Tshivenda")]
        Tshivenda,

        [Display(Name = "Xitsonga")]
        Xitsonga
    }

    public enum Province
    {
        [Display(Name = "Eastern Cape")]
        EasternCape,

        [Display(Name = "Free State")]
        FreeState,

        [Display(Name = "Gauteng")]
        Gauteng,

        [Display(Name = "KwaZulu-Natal")]
        KwaZuluNatal,

        [Display(Name = "Limpopo")]
        Limpopo,

        [Display(Name = "Mpumalanga")]
        Mpumalanga,

        [Display(Name = "Northern Cape")]
        NorthernCape,

        [Display(Name = "North West")]
        NorthWest,

        [Display(Name = "Western Cape")]
        WesternCape
    }
}
