﻿using Contentful.Core.Images;
using Contentful.Core.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wyam.Common.Documents;

namespace Contentful.Wyam
{
    public static class IDocumentExtensions
    {
        public static Asset GetIncludedAsset(this IDocument doc, JToken token)
        {
            if (token["sys"] == null || token["sys"]["id"] == null)
            {
                return null;
            }

            return GetIncludedAssetById(doc, token["sys"]["id"].ToString());
        }

        public static Asset GetIncludedAssetById(this IDocument doc, string id)
        {
            var assets = doc.List<Asset>(ContentfulKeys.IncludedAssets);

            return assets.FirstOrDefault(c => c.SystemProperties.Id == id);
        }

        public static Entry<dynamic> GetIncludedEntry(this IDocument doc, JToken token)
        {
            if(token["sys"] == null || token["sys"]["id"] == null)
            {
                return null;
            }

            return GetIncludedEntryById(doc, token["sys"]["id"].ToString());
        }

        public static Entry<dynamic> GetIncludedEntryById(this IDocument doc, string id)
        {
            var entries = doc.List<Entry<dynamic>>(ContentfulKeys.IncludedEntries);

            return entries.FirstOrDefault(c => c.SystemProperties.Id == id);
        }

        public static string ImageTagForAsset(this IDocument doc, JToken token, string alt = null,
            int? width = null, int? height = null, int? jpgQuality = null, ImageResizeBehaviour resizeBehaviour = ImageResizeBehaviour.Default,
            ImageFormat format = ImageFormat.Default, int? cornerRadius = 0, ImageFocusArea focus = ImageFocusArea.Default, string backgroundColor = null)
        {
            if (token["sys"] == null || token["sys"]["id"] == null)
            {
                return null;
            }

            return ImageTagForAsset(doc, token["sys"]["id"].ToString(), alt, width, height, jpgQuality, resizeBehaviour, format, cornerRadius, focus, backgroundColor);
        }

        public static string ImageTagForAsset(this IDocument doc, string assetId, string alt=null, 
            int? width = null, int? height = null, int? jpgQuality = null, ImageResizeBehaviour resizeBehaviour = ImageResizeBehaviour.Default, 
            ImageFormat format = ImageFormat.Default, int? cornerRadius = 0, ImageFocusArea focus = ImageFocusArea.Default, string backgroundColor = null)
        {
            var asset = doc.List<Asset>(ContentfulKeys.IncludedAssets)?.FirstOrDefault(c => c.SystemProperties.Id == assetId);
            
            if(asset == null)
            {
                return string.Empty;
            }

            var locale = doc.Get<string>(ContentfulKeys.EntryLocale);

            var imageUrlBuilder = ImageUrlBuilder.New();

            if (width.HasValue)
            {
                imageUrlBuilder.SetWidth(width.Value);
            }

            if (height.HasValue)
            {
                imageUrlBuilder.SetHeight(height.Value);
            }

            if (jpgQuality.HasValue)
            {
                imageUrlBuilder.SetJpgQuality(jpgQuality.Value);
            }

            if (cornerRadius.HasValue)
            {
                imageUrlBuilder.SetCornerRadius(cornerRadius.Value);
            }

            imageUrlBuilder.SetResizingBehaviour(resizeBehaviour).SetFormat(format).SetFocusArea(focus).SetBackgroundColor(backgroundColor);

            if(alt == null && !string.IsNullOrEmpty(asset.TitleLocalized[locale]))
            {
                alt = asset.TitleLocalized[locale];
            }

            return $@"<img src=""{asset.FilesLocalized[locale].Url + imageUrlBuilder.Build()}"" alt=""{alt}"" height=""{height}"" width=""{width}"" />";
        }

    }
}
