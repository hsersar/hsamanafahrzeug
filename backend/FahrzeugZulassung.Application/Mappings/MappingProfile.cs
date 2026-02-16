using AutoMapper;
using FahrzeugZulassung.Application.DTOs.Auftraege;
using FahrzeugZulassung.Application.DTOs.Kunden;
using FahrzeugZulassung.Application.DTOs.Mitarbeiter;
using FahrzeugZulassung.Application.DTOs.Rechnungen;
using FahrzeugZulassung.Application.DTOs.Standorte;
using FahrzeugZulassung.Domain.Entities;
using FahrzeugZulassung.Domain.Enums;

namespace FahrzeugZulassung.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Benutzer mappings
        CreateMap<Benutzer, MitarbeiterResponseDto>()
            .ForMember(dest => dest.Aktiv, opt => opt.MapFrom(src => src.IstAktiv))
            .ForMember(dest => dest.AktualisiertAm, opt => opt.MapFrom(src => src.GeaendertAm))
            .ForMember(dest => dest.Telefon, opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.Position, opt => opt.MapFrom(src => src.Rolle.ToString()))
            .ForMember(dest => dest.Standort, opt => opt.MapFrom(src => src.Standort));

        CreateMap<Benutzer, MitarbeiterInfoDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));

        // Kunde mappings
        CreateMap<Kunde, KundeResponseDto>()
            .ForMember(dest => dest.AktualisiertAm, opt => opt.MapFrom(src => src.GeaendertAm))
            .ForMember(dest => dest.Bemerkungen, opt => opt.Ignore());

        CreateMap<KundeCreateDto, Kunde>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.BenutzerId, opt => opt.Ignore())
            .ForMember(dest => dest.Benutzer, opt => opt.Ignore())
            .ForMember(dest => dest.Hausnummer, opt => opt.Ignore())
            .ForMember(dest => dest.Geburtsdatum, opt => opt.Ignore())
            .ForMember(dest => dest.DatenschutzAkzeptiert, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.DatenschutzAkzeptiertAm, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.ErstelltAm, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.GeaendertAm, opt => opt.Ignore())
            .ForMember(dest => dest.Auftraege, opt => opt.Ignore());

        CreateMap<Kunde, KundeInfoDto>();

        // Standort mappings
        CreateMap<Standort, StandortResponseDto>()
            .ForMember(dest => dest.Aktiv, opt => opt.MapFrom(src => src.IstAktiv))
            .ForMember(dest => dest.AktualisiertAm, opt => opt.MapFrom(src => src.GeaendertAm));

        CreateMap<StandortCreateDto, Standort>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Hausnummer, opt => opt.Ignore())
            .ForMember(dest => dest.IstAktiv, opt => opt.MapFrom(src => src.Aktiv))
            .ForMember(dest => dest.ErstelltAm, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.GeaendertAm, opt => opt.Ignore())
            .ForMember(dest => dest.Mitarbeiter, opt => opt.Ignore())
            .ForMember(dest => dest.Auftraege, opt => opt.Ignore());


        CreateMap<Standort, DTOs.Mitarbeiter.StandortInfoDto>();

        // Auftrag mappings
        CreateMap<Auftrag, AuftragResponseDto>()
            .ForMember(dest => dest.Typ, opt => opt.MapFrom(src => src.Typ.ToString()))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Preis, opt => opt.MapFrom(src => src.Rechnungen.FirstOrDefault() != null ? src.Rechnungen.FirstOrDefault()!.Betrag : (decimal?)null))
            .ForMember(dest => dest.DatenschutzAkzeptiert, opt => opt.MapFrom(src => src.Kunde.DatenschutzAkzeptiert))
            .ForMember(dest => dest.Kunde, opt => opt.MapFrom(src => src.Kunde))
            .ForMember(dest => dest.Fahrzeug, opt => opt.MapFrom(src => src.Fahrzeug))
            .ForMember(dest => dest.Standort, opt => opt.MapFrom(src => src.Standort))
            .ForMember(dest => dest.Mitarbeiter, opt => opt.MapFrom(src => src.ErstelltVon));

        // Fahrzeug mappings
        CreateMap<Fahrzeug, FahrzeugInfoDto>();

        CreateMap<AuftragCreateDto, Kunde>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.BenutzerId, opt => opt.Ignore())
            .ForMember(dest => dest.Benutzer, opt => opt.Ignore())
            .ForMember(dest => dest.Vorname, opt => opt.MapFrom(src => src.KundeVorname))
            .ForMember(dest => dest.Nachname, opt => opt.MapFrom(src => src.KundeNachname))
            .ForMember(dest => dest.Strasse, opt => opt.MapFrom(src => src.KundeStrasse))
            .ForMember(dest => dest.Hausnummer, opt => opt.Ignore())
            .ForMember(dest => dest.PLZ, opt => opt.MapFrom(src => src.KundePLZ))
            .ForMember(dest => dest.Ort, opt => opt.MapFrom(src => src.KundeOrt))
            .ForMember(dest => dest.Geburtsdatum, opt => opt.Ignore())
            .ForMember(dest => dest.Telefon, opt => opt.MapFrom(src => src.KundeTelefon))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.KundeEmail))
            .ForMember(dest => dest.DatenschutzAkzeptiert, opt => opt.MapFrom(src => src.DatenschutzAkzeptiert))
            .ForMember(dest => dest.DatenschutzAkzeptiertAm, opt => opt.MapFrom(src => src.DatenschutzAkzeptiert ? DateTime.UtcNow : (DateTime?)null))
            .ForMember(dest => dest.ErstelltAm, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.GeaendertAm, opt => opt.Ignore())
            .ForMember(dest => dest.Auftraege, opt => opt.Ignore());

        CreateMap<AuftragCreateDto, Fahrzeug>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.FIN, opt => opt.MapFrom(src => src.FahrzeugFIN))
            .ForMember(dest => dest.Kennzeichen, opt => opt.MapFrom(src => src.FahrzeugKennzeichen))
            .ForMember(dest => dest.Marke, opt => opt.MapFrom(src => src.FahrzeugMarke))
            .ForMember(dest => dest.Modell, opt => opt.MapFrom(src => src.FahrzeugModell))
            .ForMember(dest => dest.Erstzulassung, opt => opt.MapFrom(src => src.FahrzeugErstzulassung))
            .ForMember(dest => dest.Farbe, opt => opt.Ignore())
            .ForMember(dest => dest.Hubraum, opt => opt.Ignore())
            .ForMember(dest => dest.Leistung, opt => opt.Ignore())
            .ForMember(dest => dest.Kraftstoffart, opt => opt.Ignore())
            .ForMember(dest => dest.ErstelltAm, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.GeaendertAm, opt => opt.Ignore())
            .ForMember(dest => dest.Auftraege, opt => opt.Ignore());

        // Rechnung mappings
        CreateMap<Rechnung, RechnungResponseDto>()
            .ForMember(dest => dest.Rechnungsnummer, opt => opt.MapFrom(src => src.RechnungNummer))
            .ForMember(dest => dest.Rechnungsdatum, opt => opt.MapFrom(src => src.ErstelltAm))
            .ForMember(dest => dest.Faelligkeitsdatum, opt => opt.MapFrom(src => src.Faellig))
            .ForMember(dest => dest.Beschreibung, opt => opt.Ignore())
            .ForMember(dest => dest.IstBezahlt, opt => opt.MapFrom(src => src.Status == RechnungStatus.Bezahlt))
            .ForMember(dest => dest.Auftrag, opt => opt.MapFrom(src => src.Auftrag))
            .ForMember(dest => dest.Positionen, opt => opt.Ignore());

        CreateMap<Auftrag, AuftragInfoDto>()
            .ForMember(dest => dest.Typ, opt => opt.MapFrom(src => src.Typ.ToString()))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.KundeName, opt => opt.MapFrom(src => $"{src.Kunde.Vorname} {src.Kunde.Nachname}"));

        // Enum to string mappings
        CreateMap<AuftragTyp, string>().ConvertUsing(e => e.ToString());
        CreateMap<string, AuftragTyp>().ConvertUsing(s => Enum.Parse<AuftragTyp>(s, true));
        
        CreateMap<AuftragStatus, string>().ConvertUsing(e => e.ToString());
        CreateMap<string, AuftragStatus>().ConvertUsing(s => Enum.Parse<AuftragStatus>(s, true));
        
        CreateMap<RechnungStatus, string>().ConvertUsing(e => e.ToString());
        CreateMap<string, RechnungStatus>().ConvertUsing(s => Enum.Parse<RechnungStatus>(s, true));
        
        CreateMap<BenutzerRolle, string>().ConvertUsing(e => e.ToString());
        CreateMap<string, BenutzerRolle>().ConvertUsing(s => Enum.Parse<BenutzerRolle>(s, true));
    }
}
