
using Entities.DataTransferObjects;
using Entities.Models;
using ImageMagick;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace CMS.API.InternalServices
{
    public class FileRepository : IFileRepository
    {
        private CmsContext _cmsContext;
        public IConfiguration _configuration { get; }
        private IWebHostEnvironment _iwebHostEnvironment;
        private string filePathMedia = "";
        public FileRepository(CmsContext cmsContext
                             , IConfiguration configuration
                             , IWebHostEnvironment iwebHostEnvironment
            )
        {
            _configuration = configuration;
            _iwebHostEnvironment = iwebHostEnvironment;
            _cmsContext = cmsContext;
            filePathMedia = _configuration["FilePathMedia"];
        }

        public async Task<string> AddProfilePic(IFormFile ProfileImage)
        {

            var filename = Guid.NewGuid().ToString() + "_" + (ProfileImage.FileName);
            var path = Path.Combine(_iwebHostEnvironment.WebRootPath, $"{filePathMedia}/ProfileImages", filename);
            await ResizeImage(ProfileImage, path, 200);
            //var stream = new FileStream(path, FileMode.Create);
            //await ProfileImage.CopyToAsync(stream);

            return filename;
        }

        public async Task<string> AddMediaPicAsync(IFormFile image, List<Setting> sysConfig)
        {
            bool ImageResize = bool.Parse(sysConfig.FirstOrDefault(x => x.Label == "ImageResize").Value);
            int photoLargeWidth = int.Parse(sysConfig.FirstOrDefault(x => x.Label == "PhotoLargeWidth").Value);
            int photoMediumWidth = int.Parse(sysConfig.FirstOrDefault(x => x.Label == "PhotoMediumWidth").Value);
            int photoSmallWidth = int.Parse(sysConfig.FirstOrDefault(x => x.Label == "PhotoSmallWidth").Value);

            //Large Image
            string filename = Guid.NewGuid().ToString();
            var path = Path.Combine(_iwebHostEnvironment.WebRootPath, filePathMedia, filename + Path.GetExtension(image.FileName));
            if (ImageResize)
            {
                await ResizeImage(image, path, photoLargeWidth);
            }
            else
            {
                //Original file
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }
            }


            //Medium Image
            var pathMedium = Path.Combine(_iwebHostEnvironment.WebRootPath, filePathMedia, filename + "_medium" + Path.GetExtension(image.FileName));
            await ResizeImage(image, pathMedium, photoMediumWidth);

            //Small Image
            var pathSmall = Path.Combine(_iwebHostEnvironment.WebRootPath, filePathMedia, filename + "_small" + Path.GetExtension(image.FileName));
            await ResizeImage(image, pathSmall, photoSmallWidth);

            return filename;
        }

        public async Task<string> AddMediaPicWithCropAsync(IFormFile image, List<Setting> sysConfig, MediaDto model)
        {

            int photoLargeWidth = int.Parse(sysConfig.FirstOrDefault(x => x.Label == "PhotoLargeWidth").Value);
            int photoLargeHeigth = int.Parse(sysConfig.FirstOrDefault(x => x.Label == "PhotoLargeHeight").Value);
            int photoMediumWidth = int.Parse(sysConfig.FirstOrDefault(x => x.Label == "PhotoMediumWidth").Value);
            int photoSmallWidth = int.Parse(sysConfig.FirstOrDefault(x => x.Label == "PhotoSmallWidth").Value);

            if (model.CropWidth == null)
            {
                model.CropWidth = photoLargeWidth;
            }
            if (model.CropHeight == null)
            {
                model.CropHeight = photoLargeHeigth;
            }

            string filename = Guid.NewGuid().ToString();
            var path = Path.Combine(_iwebHostEnvironment.WebRootPath, filePathMedia, filename + Path.GetExtension(image.FileName));
            if (model.IsCrop == true && model.Resize == true)
            {
                MemoryStream stream = new MemoryStream();
                await image.CopyToAsync(stream);
                stream.Position = 0;
                var img = Image.FromStream(stream);
                var thumbImage = CropImage(img, new Size(model.CropWidth.Value, model.CropHeight.Value));

                //Larg Image
                await ResizeImageCrop(thumbImage, path, model.CropWidth.Value);

                //Medium Image
                var pathMedium = Path.Combine(_iwebHostEnvironment.WebRootPath, filePathMedia, filename + "_medium" + Path.GetExtension(image.FileName));
                await ResizeImageCrop(thumbImage, pathMedium, model.CropWidth.Value / 2);

                //Small Image
                var pathSmall = Path.Combine(_iwebHostEnvironment.WebRootPath, filePathMedia, filename + "_small" + Path.GetExtension(image.FileName));
                await ResizeImageCrop(thumbImage, pathSmall, model.CropWidth.Value / 4);

                return filename;
            }
            else if (model.IsCrop == true && model.Resize == false)
            {
                MemoryStream stream = new MemoryStream();
                await image.CopyToAsync(stream);
                stream.Position = 0;
                var img = Image.FromStream(stream);
                var thumbImage = CropImage(img, new Size(model.CropWidth.Value, model.CropHeight.Value));
                thumbImage.Save(path);

                //Medium Image
                var pathMedium = Path.Combine(_iwebHostEnvironment.WebRootPath, filePathMedia, filename + "_medium" + Path.GetExtension(image.FileName));
                await ResizeImageCrop(thumbImage, pathMedium, model.CropWidth.Value / 2);

                //Small Image
                var pathSmall = Path.Combine(_iwebHostEnvironment.WebRootPath, filePathMedia, filename + "_small" + Path.GetExtension(image.FileName));
                await ResizeImageCrop(thumbImage, pathSmall, model.CropWidth.Value / 4);

                return filename;
            }
            else if (model.IsCrop == false && model.Resize == true)
            {
                await ResizeImage(image, path, photoLargeWidth);
                //Medium Image
                var pathMedium = Path.Combine(_iwebHostEnvironment.WebRootPath, filePathMedia, filename + "_medium" + Path.GetExtension(image.FileName));
                await ResizeImage(image, pathMedium, photoMediumWidth);

                //Small Image
                var pathSmall = Path.Combine(_iwebHostEnvironment.WebRootPath, filePathMedia, filename + "_small" + Path.GetExtension(image.FileName));
                await ResizeImage(image, pathSmall, photoSmallWidth);

                return filename;
            }
            else
            {
                //Original file
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                //Medium file
                var pathMedium = Path.Combine(_iwebHostEnvironment.WebRootPath, filePathMedia, filename + "_medium" + Path.GetExtension(image.FileName));
                //await ResizeImage(image, pathMedium, photoSmallWidth);
                using (var stream = new FileStream(pathMedium, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }


                //Small Image for list
                var pathSmall = Path.Combine(_iwebHostEnvironment.WebRootPath, filePathMedia, filename + "_small" + Path.GetExtension(image.FileName));
                //await ResizeImage(image, pathSmall, photoSmallWidth);
                using (var stream = new FileStream(pathSmall, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }


                return filename;
            }
        }

        public async Task<string> AddFileAndVideo(IFormFile file, int mediaExCategoryId)
        {

            string filename = Guid.NewGuid().ToString();
            var path = Path.Combine(_iwebHostEnvironment.WebRootPath, filePathMedia, filename + Path.GetExtension(file.FileName));

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return filename;
        }

        public bool DeleteFile(Medium media)
        {
            try
            {
                if (media.MediaExCategoryId == 1) //Foto
                {
                    var path = Path.Combine(_iwebHostEnvironment.WebRootPath, filePathMedia, media.FileName + media.FileEx);
                    FileInfo file = new(path);
                    if (file.Exists)
                    {
                        file.Delete();
                    }

                    var pathMedium = Path.Combine(_iwebHostEnvironment.WebRootPath, filePathMedia, media.FileNameMedium + media.FileEx);
                    FileInfo fileMedium = new(pathMedium);
                    if (fileMedium.Exists)
                    {
                        fileMedium.Delete();
                    }

                    var pathSmall = Path.Combine(_iwebHostEnvironment.WebRootPath, filePathMedia, media.FileNameSmall + media.FileEx);
                    FileInfo fileSmall = new(pathSmall);
                    if (fileSmall.Exists)
                    {
                        fileSmall.Delete();
                    }
                }
                else //Video and Doc
                {
                    var path = Path.Combine(_iwebHostEnvironment.WebRootPath, filePathMedia, media.FileName + media.FileEx);
                    FileInfo file = new(path);
                    if (file.Exists)
                    {
                        file.Delete();
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        protected async Task ResizeImage(IFormFile file, string filePath, int size)
        {
            MemoryStream stream = new MemoryStream();
            await file.CopyToAsync(stream);
            stream.Position = 0;

            int width, height;
            var img = Image.FromStream(stream);

            if (img.Width > img.Height)
            {
                width = size;
                height = Convert.ToInt32(img.Height * size / (double)img.Width);
            }
            else
            {
                width = Convert.ToInt32(img.Width * size / (double)img.Height);
                height = size;
            }

            stream.Position = 0;
            var resizer = new MagickImage(stream);

            resizer.Resize(width, height);
            //resizer.Orientation = OrientationType.TopLeft;

            resizer.Write(filePath);
        }

        #region Image Crop and Resize

        public async Task<string> CropImages(IFormFile image, int width, int height)
        {
            string filename = Guid.NewGuid().ToString();
            var path = Path.Combine(_iwebHostEnvironment.WebRootPath, filePathMedia, filename + Path.GetExtension(image.FileName));

            MemoryStream stream = new MemoryStream();
            await image.CopyToAsync(stream);
            stream.Position = 0;
            var img = Image.FromStream(stream);
            var thumbImage = CropImage(img, new Size(width, height));
            //thumbImage.Save(path);

            await ResizeImageCrop(thumbImage, path, width);

            //Medium Image
            var pathMedium = Path.Combine(_iwebHostEnvironment.WebRootPath, filePathMedia, filename + "_medium" + Path.GetExtension(image.FileName));
            await ResizeImageCrop(thumbImage, pathMedium, width / 2);

            //Small Image
            var pathSmall = Path.Combine(_iwebHostEnvironment.WebRootPath, filePathMedia, filename + "_small" + Path.GetExtension(image.FileName));
            await ResizeImageCrop(thumbImage, pathSmall, width / 4);

            return filename;
        }

        protected Image CropImage(Image imgToResize, Size destinationSize)
        {
            var originalWidth = imgToResize.Width;
            var originalHeight = imgToResize.Height;

            //how many units are there to make the original length
            var hRatio = (float)originalHeight / destinationSize.Height;
            var wRatio = (float)originalWidth / destinationSize.Width;

            //get the shorter side
            var ratio = Math.Min(hRatio, wRatio);

            var hScale = Convert.ToInt32(destinationSize.Height * ratio);
            var wScale = Convert.ToInt32(destinationSize.Width * ratio);

            //start cropping from the center
            var startX = (originalWidth - wScale) / 2;
            var startY = (originalHeight - hScale) / 2;

            //crop the image from the specified location and size
            var sourceRectangle = new Rectangle(startX, startY, wScale, hScale);

            //the future size of the image
            var bitmap = new Bitmap(destinationSize.Width, destinationSize.Height);

            //fill-in the whole bitmap
            var destinationRectangle = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            //generate the new image
            using (var g = Graphics.FromImage(bitmap))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(imgToResize, destinationRectangle, sourceRectangle, GraphicsUnit.Pixel);
            }

            return bitmap;
        }

        protected async Task ResizeImageCrop(Image image, string filePath, int size)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                //image.Save(stream, image.RawFormat);
                image.Save(stream, ImageFormat.Bmp);
                stream.Position = 0;

                int width, height;
                if (image.Width > image.Height)
                {
                    width = size;
                    height = Convert.ToInt32(image.Height * size / (double)image.Width);
                }
                else
                {
                    width = Convert.ToInt32(image.Width * size / (double)image.Height);
                    height = size;
                }

                using (var resizer = new MagickImage(stream))
                {
                    resizer.Resize(width, height);
                    //resizer.Orientation = OrientationType.TopLeft;
                    resizer.Write(filePath);
                }
            }
        }

        #endregion
    }
}
