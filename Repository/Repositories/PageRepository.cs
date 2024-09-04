using Entities.Models;
using Contracts.IServices;
using Entities.DataTransferObjects;
using Entities.RequestFeatures;
using Microsoft.EntityFrameworkCore;
using Repository.Extensions;
using Entities.DataTransferObjects.WebDTOs;

namespace Repository.Repositories
{
	public class PageRepository : GenericRepository<Page>, IPageRepository
	{
		public PageRepository(CmsContext cmsContext
			 ) : base(cmsContext)
		{

		}

		public async Task<PagedList<Page>> GetPagesAsync(PageFilterParameters parameter)
		{
			IQueryable<Page> data = _cmsContext.Pages.Include(x => x.Layout).Include(x => x.Template).Include(x => x.Media).Include(t => t.InversePageNavigation).IgnoreAutoIncludes().AsNoTracking()
											.FilterPageByLayout(parameter.LayoutId)
											.FilterPageByLanguage(parameter.webLangId)
											.FilterPageByTemplateId(parameter.TemplateId)
											.FilterPageDeleted(parameter.isDeleted)
											.FilterPageByParentId(parameter.parentPageId)
											.Search(parameter.Query)
											.Sort(parameter.Sort.key + " " + parameter.Sort.order);

			return PagedList<Page>
			.ToPagedList(data, parameter.PageIndex,
			parameter.PageSize);
		}

		public async Task<PagedList<Page>> GetSubPagesAsync(PageFilterParameters parameter)
		{
			IQueryable<Page> data = _cmsContext.Pages.Include(x => x.Layout).Include(x => x.Template).Include(x => x.Media).Include(t => t.InversePageNavigation).IgnoreAutoIncludes().AsNoTracking()
											//.FilterPageByLayout(parameter.LayoutId)
											.FilterPageByLanguage(parameter.webLangId)
											.FilterPageByTemplateId(parameter.TemplateId)
											.FilterPageDeleted(parameter.isDeleted)
											.FilterPageByParentId(parameter.parentPageId)
											.Search(parameter.Query)
											.Sort(parameter.Sort.key + " " + parameter.Sort.order);

			return PagedList<Page>
			.ToPagedList(data, parameter.PageIndex,
			parameter.PageSize);
		}

		public async Task<PagedList<Page>> GetSubPagesTrashAsync(SubPageFilterParameters parameter)
		{
			IQueryable<Page> data = _cmsContext.Pages.Include(x => x.Layout).Include(x => x.Template).Include(x => x.Media).Include(t => t.InversePageNavigation).IgnoreAutoIncludes().AsNoTracking()
											.FilterPageByLanguage(parameter.webLangId)
											.FilterPageByTemplateId(parameter.TemplateId)
											.FilterPageDeleted(parameter.isDeleted)
											.FilterSubPageByParentId(parameter.SubPageParentId)
											.FilterIsSubPage()
											.Search(parameter.Query)
											.Sort(parameter.Sort.key + " " + parameter.Sort.order);

			return PagedList<Page>
			.ToPagedList(data, parameter.PageIndex,
			parameter.PageSize);
		}


		public async Task<Page> GetPageById(int id, int webLangId)
		{
			var page = await _cmsContext.Pages.FindAsync(id, webLangId);
			return page;
		}

		public async Task<bool> AddMediaCollectionInPage(AddMediaCollectionInPage model, List<Language> langList, string UserId)
		{
			try
			{
				if (model.WebMultiLang)
				{
					foreach (var item in model.MediaId)
					{
						foreach (var lang in langList)
						{
							bool existsPageInCurrentLang = _cmsContext.Pages.Where(x => x.Id == model.PageId && x.LanguageId == lang.Id && x.Deleted != true).Any();
							bool existsMediaInThisPage = _cmsContext.PageMedia.Where(x => x.PageId == model.PageId && x.MediaId == item && x.LanguageId == lang.Id).Any();
							var media = await _cmsContext.Media.Where(x => x.Id == item).FirstOrDefaultAsync();
							if (existsPageInCurrentLang && (!existsMediaInThisPage)) //check if posts exists in current language
							{
								var postMediaEntity = new PageMedium()
								{
									MediaId = item,
									PageId = model.PageId,
									Name = media.Name,
									IsSlider = model.IsSlider,
									LanguageId = lang.Id,
									StartDate = DateTime.Now,
									CreatedBy = UserId,
									Created = DateTime.Now,
								};

								_cmsContext.PageMedia.Add(postMediaEntity);
								await _cmsContext.SaveChangesAsync();
							}

						}
					}
				}
				else
				{
					foreach (var item in model.MediaId)
					{
						var media = await _cmsContext.Media.Where(x => x.Id == item).FirstOrDefaultAsync();
						bool existsMediaInThisPage = _cmsContext.PageMedia.Where(x => x.PageId == model.PageId && x.MediaId == item && x.LanguageId == model.webLangId).Any();
						if (!existsMediaInThisPage)
						{
							var postMediaEntity = new PageMedium()
							{
								MediaId = item,
								PageId = model.PageId,
								Name = media.Name,
								IsSlider = model.IsSlider,
								LanguageId = model.webLangId,
                                StartDate = DateTime.Now,
                                CreatedBy = UserId,
								Created = DateTime.Now,
							};
							_cmsContext.PageMedia.Add(postMediaEntity);
							await _cmsContext.SaveChangesAsync();
						}
					}
				}

				return true;
			}
			catch (Exception)
			{
				return false;
			}

		}

		public async Task<bool> RemoveMediaCollectionFromPage(List<int> MediaIds, List<Language> langList, bool WebMultiLang, int webLangId, int pageId)
		{
			if (MediaIds.Count > 0)
			{
				foreach (int Id in MediaIds)
				{
					if (WebMultiLang)
					{
						foreach (var lang in langList)
						{
							var mediaInPage = await _cmsContext.PageMedia.Where(x => x.PageId == pageId && x.MediaId == Id && x.LanguageId == lang.Id).FirstOrDefaultAsync();
							if (mediaInPage != null)
							{
								_cmsContext.Remove(mediaInPage);
								await _cmsContext.SaveChangesAsync();
							}
						}
					}
					else
					{
						var mediaInPage = await _cmsContext.PageMedia.Where(x => x.PageId == pageId && x.MediaId == Id && x.LanguageId == webLangId).FirstOrDefaultAsync();
						if (mediaInPage != null)
						{
							_cmsContext.Remove(mediaInPage);
							await _cmsContext.SaveChangesAsync();
						}
					}

				}
			}
			return true;
		}

		public async Task<List<GetPageMediaDto>> GetPageMedia(int pageId, int webLangId, bool isSlider)
		{
			var list = await _cmsContext.PageMedia.Include(x => x.Media).ThenInclude(x => x.FileExNavigation).Where(x => x.PageId == pageId && x.LanguageId == webLangId && x.IsSlider == isSlider)
					.Select(t => new GetPageMediaDto
					{
						PageId = t.PageId,
						LanguageId = t.LanguageId,
						MediaId = t.MediaId,
						OrderNo = t.OrderNo,
						CreatedBy = t.CreatedBy,
						Name = t.Name,
						StartDate = t.StartDate.Value.ToString("dd/MM/yyyy"),
						EndDate = t.EndDate.Value.ToString("dd/MM/yyyy"),
						Media = t.Media
					}).OrderBy(x => x.OrderNo).ToListAsync();
			return list;
		}


		#region web
		public async Task<List<PageModel>> GetBasicPage(int PageId, int GjuhaId, DateTime formatedDateTime)
		{
			var pageLista = await _cmsContext.Pages.Include(t => t.Media).Where(t => t.LanguageId == GjuhaId && t.Id == PageId && t.Deleted != true)
					.Select(p => new PageModel()
					{
						PageId = p.Id,
						GjuhaId = p.LanguageId,
						PageAdresa = p.PageAdress,
						PageName = p.PageName,
						PageText = p.PageText,
						MediaId = p.MediaId,
						Media = p.Media,
						TemplateId = p.TemplateId,
						Template = p.Template,
						Layout = p.Layout,
						LayoutId = p.LayoutId,
						hasMedia = (from pm in _cmsContext.PageMedia

												where pm.LanguageId == GjuhaId && pm.PageId == PageId && pm.IsSlider == false
																			&& (pm.StartDate != null ? pm.StartDate.Value <= formatedDateTime : true)
																			&& (pm.EndDate != null ? pm.EndDate.Value >= formatedDateTime : true)
												select pm).Count() > 0,
						hasSlider = (from pm in _cmsContext.PageMedia

												 where pm.LanguageId == GjuhaId && pm.PageId == PageId && pm.IsSlider == true
																			 && (pm.StartDate != null ? pm.StartDate.Value <= formatedDateTime : true)
																			 && (pm.EndDate != null ? pm.EndDate.Value >= formatedDateTime : true)
												 select pm).Count() > 0,
						//hasMedia = _cmsContext.CmsPageMedia.Where(t=>t.PageId == p.PageId && t.GjuhaId == GjuhaId
						//                        && (t.DataFillimit != null ? t.DataFillimit.Value <= formattedDtm : true)
						//                        && (t.DataMbarimit != null ? t.DataMbarimit.Value >= formattedDtm : true)).Count() > 0,
						HasSubPages = _cmsContext.Pages.Where(t => t.PageParentId == p.Id && t.LanguageId == GjuhaId && t.Deleted != true).Count() > 0,
						Fshire = p.Deleted,
					}).ToListAsync();

			return pageLista;
		}

		public async Task<List<PageModel>> GetSubPages(int PageId, int GjuhaId, DateTime formatedDateTime, int? top)
		{
			var pageLista = new List<PageModel>();

			if (top > 0)
			{
				pageLista = await _cmsContext.Pages.Where(t => t.LanguageId == GjuhaId && t.PageParentId == PageId && t.Deleted != true)
						.Select(p => new PageModel()
						{
							PageId = p.Id,
							GjuhaId = p.LanguageId,
							PageName = p.PageName,
							Fshire = p.Deleted,
						}).OrderByDescending(x => x.PageId).Take((int)top).ToListAsync();
			}
			else
			{
				pageLista = await _cmsContext.Pages.Where(t => t.LanguageId == GjuhaId && t.PageParentId == PageId && t.Deleted != true)
						.Select(p => new PageModel()
						{
							PageId = p.Id,
							GjuhaId = p.LanguageId,
							PageName = p.PageName,
							Fshire = p.Deleted,
						}).ToListAsync();
			}
			return pageLista;
		}

		public async Task<object> GetPageMedia(int PageId, int GjuhaId, int skip, int take, int Viti, string searchText, DateTime formatedDateTime)
		{
			var mediaLista = await (from pm in _cmsContext.PageMedia.Include(t => t.Media).ThenInclude(t => t.FileExNavigation)

															where pm.LanguageId == GjuhaId && pm.PageId == PageId
																			&& pm.IsSlider == false
																			&& (pm.StartDate != null ? pm.StartDate.Value <= formatedDateTime : true)
																			&& (pm.EndDate != null ? pm.EndDate.Value >= formatedDateTime : true)
																			&& (Viti > 0 ? pm.StartDate.Value.Year == Viti : true)
																			&& (!string.IsNullOrEmpty(searchText) ? pm.Name.Contains(searchText) : true)
															select new
															{
																pageMedia = pm,
																media = pm.Media,
																fileIco = pm.Media.FileExNavigation == null && pm.Media.MediaExCategoryId == 3 ? _cmsContext.MediaExes.Where(t => t.MediaEx1 == ".pdf ").FirstOrDefault() : pm.Media.FileExNavigation,
															}).OrderBy(t => t.pageMedia.OrderNo).Skip(skip).Take(take).ToListAsync();

			return mediaLista;
		}

		public async Task<object> GetPageSlider(int PageId, int GjuhaId)
		{
			var mediaLista = await (from pm in _cmsContext.PageMedia.Include(t => t.Media)

															where pm.LanguageId == GjuhaId && pm.PageId == PageId
																			&& pm.IsSlider == true
															select new
															{
																pageMedia = pm,
																media = pm.Media,
															}).OrderBy(t => t.pageMedia.OrderNo).ToListAsync();

			return mediaLista;
		}

		public async Task<int> GetPageMediaCount(int PageId, int GjuhaId, int Viti, string searchText, DateTime formatedDateTime)
		{
			var pageCount = await (from pm in _cmsContext.PageMedia.Include(t => t.Media).ThenInclude(t => t.FileExNavigation)

														 where pm.LanguageId == GjuhaId && pm.PageId == PageId
																		 && pm.IsSlider == false
																		 && (pm.StartDate != null ? pm.StartDate.Value <= formatedDateTime : true)
																		 && (pm.EndDate != null ? pm.EndDate.Value >= formatedDateTime : true)
																		 && (Viti > 0 ? pm.StartDate.Value.Year == Viti : true)
																		 && (!string.IsNullOrEmpty(searchText) ? pm.Name.Contains(searchText) : true)
														 select pm
														 ).CountAsync();

			return pageCount;
		}

		public async Task<List<VitetModel>> GetVitet(int PageId, int GjuhaId, DateTime formatedDateTime)
		{
            //var vitetLista = await (from pm in _cmsContext.PageMedia

            //						where pm.LanguageId == GjuhaId && pm.PageId == PageId
            //									 && pm.IsSlider == false
            //									 && (pm.StartDate != null ? pm.StartDate.Value <= formatedDateTime : true)
            //									 && (pm.EndDate != null ? pm.EndDate.Value >= formatedDateTime : true)
            //						select new VitetModel
            //						{
            //							Viti = pm.StartDate.Value.Year
            //						}).Distinct().OrderByDescending(x => x.Viti).ToListAsync();

            //return vitetLista;
            var vitetLista = await (from pm in _cmsContext.PageMedia
                                    where pm.LanguageId == GjuhaId
                                          && pm.PageId == PageId
                                          && !pm.IsSlider
                                          && (!pm.StartDate.HasValue || pm.StartDate.Value <= formatedDateTime)
                                          && (!pm.EndDate.HasValue || pm.EndDate.Value >= formatedDateTime)
                                    select new VitetModel
                                    {
                                        Viti = pm.StartDate.HasValue ? pm.StartDate.Value.Year : pm.Created.Year 
                                    }).Distinct().OrderByDescending(x => x.Viti).ToListAsync();

            return vitetLista;
        }
		public int? GetPageIdFromPostimiGategoriaID(int PostimiKategoriaID, int GjuhaId)
		{
			int? pageId = 0;
			var kategoria = _cmsContext.PostCategories.Where(x => x.Id == PostimiKategoriaID && x.LanguageId == GjuhaId).FirstOrDefault();

			if (kategoria != null)
			{
				pageId = kategoria.PageId;
			}

			return pageId;
		}
		#endregion
	}
}
