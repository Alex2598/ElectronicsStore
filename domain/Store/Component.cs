
using Store.Data;
using System;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
namespace Store
{
    public class Component
    {
        private readonly ComponentDto dto;

        public int Id => dto.Id;

        public string UId
        {
            get => dto.UId;
            set
            {
                if (TryFormatUId(value, out string formattedIsbn))
                    dto.UId = formattedIsbn;

                throw new ArgumentException(nameof(UId));
            }
        }

        public string Package
        {
            get => dto.Package;
            set => dto.Package = value?.Trim();
        }

        public string NameOfComponent
        {
            get => dto.NameOfComponent;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException(nameof(NameOfComponent));

                dto.NameOfComponent = value.Trim();
            }
        }

        public string Description
        {
            get => dto.Description;
            set => dto.Description = value;
        }

        public decimal Price
        {
            get => dto.Price;
            set => dto.Price = value;
        }

        internal Component(ComponentDto dto)
        {
            this.dto = dto;
        }

        public static bool TryFormatUId(string uid, out string formattedUid)
        {
            if (uid == null)
            {
                formattedUid = null;
                return false;
            }

            formattedUid = uid.
                                ToUpper();

            return Regex.IsMatch(formattedUid, @"^\d{5}?$");
        }

        public static bool IsUId(string uid)
            => TryFormatUId(uid, out _);

        public static class DtoFactory
        {
            public static ComponentDto Create(string uid,
                                         string package,
                                         string nameofcomponent,
                                         string description,
                                         decimal price)
            {
                if (TryFormatUId(uid, out string formattedUid))
                    uid = formattedUid;
                else
                    throw new ArgumentException(nameof(uid));

                if (string.IsNullOrWhiteSpace(nameofcomponent))
                    throw new ArgumentException(nameof(nameofcomponent));

                return new ComponentDto
                {
                    UId = uid,
                    Package = package?.Trim(),
                    NameOfComponent = nameofcomponent.Trim(),
                    Description = description?.Trim(),
                    Price = price,
                };
            }
        }

        public static class Mapper
        {
            public static Component Map(ComponentDto dto) => new Component(dto);

            public static ComponentDto Map(Component domain) => domain.dto;
        }
    }
}