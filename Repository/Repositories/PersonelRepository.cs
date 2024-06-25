using Entities.Models;
using Contracts.IServices;
using Entities.RequestFeatures;
using Microsoft.EntityFrameworkCore;
using Repository.Extensions;
using Entities.DataTransferObjects.WebDTOs;
using System;

namespace Repository.Repositories
{
	public class PersonelRepository : GenericRepository<Personel>, IPersonelRepository
	{
		public PersonelRepository(CmsContext cmsContext) : base(cmsContext)
		{
		}

		public async Task<Personel?> GetPersonelById(int id, int webLangId)
		{
			var personel = await _cmsContext.Personels.Include(x => x.Media).Where(x => x.Id == id && x.LanguageId == webLangId).FirstOrDefaultAsync();

			return personel;

		}

		public async Task<PagedList<Personel>> GetPersonelAsync(PersonelFilterParameters parameter)
		{
			IQueryable<Personel> data = _cmsContext.Personels.Include(x => x.Layout).Include(x => x.Page).Include(x => x.Media).IgnoreAutoIncludes().AsNoTracking()
											.FilterPersonelByLanguage(parameter.webLangId)
											.FilterPersonelByLayout(parameter.LayoutId)
											.FilterPersonelByPage(parameter.PageId)
											.Search(parameter.Query)
											.Sort(parameter.Sort.key + " " + parameter.Sort.order);
			//.ToListAsync();

			return PagedList<Personel>
			.ToPagedList(data, parameter.PageIndex,
			parameter.PageSize);
		}

		#region Web
		public async Task<List<PersonelModel>> GetPersonel(int GjuhaId, int PageId, int skip, int take)
		{
			var contactlLista = await (from k in _cmsContext.Personels.Include(t => t.Page).ThenInclude(t => t.Layout)

																 where k.LanguageId == GjuhaId && k.PageId == PageId && k.Active != false
																 select new PersonelModel
																 {
																	 PersoneliId = k.Id,
																	 PersoneliEmri = k.Name,
																	 PersoneliMbiemri = k.LastName,
																	 PersoneliEmail = k.Email,
																	 PersoneliDataLindjes = k.BirthDate,
																	 PersoneliNrTelefonit = k.PhoneNumber,
																	 PersoneliPozita = k.Position,
																	 PersoneliKualifikimi = k.Qualification,
																	 PresoneliVendiLindjes = k.BirthPlace,
																	 PersoneliInformataShtes = k.OtherInfo,
																	 PersoneliOrderNr = k.OrderNo,
																	 PersoneliAktiv = k.Active,
																	 GjuhaId = k.LanguageId,
																	 PageId = k.PageId,
																	 PageName = k.Page.PageName,
																	 Url = k.Page.Layout.Path + "/PersonelDetails/" + PageId + "/" + k.Id,
																	 MediaId = k.MediaId,
																	 media = (from m in _cmsContext.Media
																						where m.Id == k.MediaId
																						select new MediaModel
																						{
																							MediaId = m.Id,
																							MediaEmri = m.FileName.ToString(),
																							MediaEmri_medium = m.FileNameMedium,
																							MediaEmri_small = m.FileNameSmall,
																							MediaEx = m.FileEx,
																							MediaPershkrimi = m.Name,
																							MediaDataInsertimit = m.Created,
																							MediaExKategoriaId = m.MediaExCategoryId,
																							IsOtherSource = m.IsOtherSource,
																							OtherSource = m.OtherSourceLink
																						}).FirstOrDefault()
																 }).OrderBy(t => t.PersoneliOrderNr).Skip(skip).Take(take).ToListAsync();

			return contactlLista;
		}

		public async Task<int> GetPersonelCount(int GjuhaId, int PageId)
		{
			int pageCount = await (from k in _cmsContext.Personels.Include(t => t.Page).ThenInclude(t => t.Layout)

														 where k.LanguageId == GjuhaId && k.PageId == PageId && k.Active != false
														 select new PersonelModel
														 {
															 PersoneliId = k.Id,
															 PersoneliEmri = k.Name,
															 PersoneliMbiemri = k.LastName,
															 PersoneliEmail = k.Email,
															 PersoneliDataLindjes = k.BirthDate,
															 PersoneliNrTelefonit = k.PhoneNumber,
															 PersoneliPozita = k.Position,
															 PersoneliKualifikimi = k.Qualification,
															 PresoneliVendiLindjes = k.BirthPlace,
															 //PersoneliInformataShtes = k.OtherInfo,
															 PersoneliOrderNr = k.OrderNo,
															 PersoneliAktiv = k.Active,
															 GjuhaId = k.LanguageId,
															 PageId = k.PageId,
															 PageName = k.Page.PageName,
															 Url = k.Page.Layout.Path + "/PersonelDetails/" + PageId + "/" + k.Id,
															 MediaId = k.MediaId
														 }).CountAsync();

			return pageCount;
		}

		public async Task<List<PersonelModel>> GetPersonelDetails(int GjuhaId, int PersoneliID)
		{
			var contactlLista = await (from k in _cmsContext.Personels.Include(t => t.Page).ThenInclude(t => t.Layout)

																 where k.LanguageId == GjuhaId && k.Id == PersoneliID && k.Active != false
																 select new PersonelModel
																 {
																	 PersoneliId = k.Id,
																	 PersoneliEmri = k.Name,
																	 PersoneliMbiemri = k.LastName,
																	 PersoneliEmail = k.Email,
																	 PersoneliDataLindjes = k.BirthDate,
																	 PersoneliNrTelefonit = k.PhoneNumber,
																	 PersoneliPozita = k.Position,
																	 PersoneliKualifikimi = k.Qualification,
																	 PresoneliVendiLindjes = k.BirthPlace,
																	 PersoneliInformataShtes = k.OtherInfo,
																	 PersoneliOrderNr = k.OrderNo,
																	 PersoneliAktiv = k.Active,
																	 GjuhaId = k.LanguageId,
																	 PageId = k.PageId,
																	 PageName = k.Page.PageName,
																	 MediaId = k.MediaId,
																	 media = (from m in _cmsContext.Media
																						where m.Id == k.MediaId
																						select new MediaModel
																						{
																							MediaId = m.Id,
																							MediaEmri = m.FileName.ToString(),
																							MediaEmri_medium = m.FileNameMedium,
																							MediaEmri_small = m.FileNameSmall,
																							MediaEx = m.FileEx,
																							MediaPershkrimi = m.Name,
																							MediaDataInsertimit = m.Created,
																							MediaExKategoriaId = m.MediaExCategoryId,
																							IsOtherSource = m.IsOtherSource,
																							OtherSource = m.OtherSourceLink
																						}).FirstOrDefault()
																 }).OrderBy(t => t.PersoneliOrderNr).ToListAsync();

			return contactlLista;
		}
		#endregion

	}
}
