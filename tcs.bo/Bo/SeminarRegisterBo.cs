using System;
using System.Collections.Generic;
using System.Linq;

namespace tcs.bo
{
    [Serializable]
    public class SeminarRegisterBo : BaseBo
    {
        public int SeminarId { get; set; }
        public string SeminarName { get; set; }
        public int SeminarPlaceId { get; set; }
        public int CustomerId { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string CustomerNote { get; set; }
        public string Note { get; set; }
        public bool IsAttend { get; set; }
        public string TicketId { get; set; }
        public string School1 { get; set; }
        public string School1Time { get; set; }
        public string School2 { get; set; }
        public string School2Time { get; set; }
        public string School3 { get; set; }
        public string School3Time { get; set; }
        public string EmployeeName { get; set; }
        public int Source { get; set; }
    }

    [Serializable]
    public class SeminarRegisterQuery : IQuery
    {
        public int SeminarId { get; set; }
        public int SeminarPlaceId { get; set; }
        public int IsAttend { get; set; }
        public int TitleId { get; set; }
    }

    [Serializable]
    public class SeminarRegisterExcel
    {
        public int CustomerId { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Birthday { get; set; }
        public string School { get; set; }
        public string Class { get; set; }
        public string EducationLevel { get; set; }
        public string Major { get; set; }
        public string StudyAbroadTime { get; set; }
        public string Financial { get; set; }
        public string GPA { get; set; }
        public string LanguageInfo { get; set; }
        public string SourceName { get; set; }
    }

    [Serializable]
    public class SeminarRegisterSE
    {
        public int Id { get; set; }
        public int SeminarId { get; set; }
        public int SeminarPlaceId { get; set; }
        public int CustomerId { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public DateTime Birthday { get; set; }
        public string ParentInfo { get; set; }
        public string School { get; set; }
        public string Class { get; set; }
        public string GPA { get; set; }
        public string EducationLevel { get; set; }
        public string Major { get; set; }
        public string StudyAbroadTime { get; set; }
        public string Financial { get; set; }
        public string LanguageInfo { get; set; }
        public int Source { get; set; }
        public string SourceName { get; set; }
        public int TitleId { get; set; }
        public string TitleName { get; set; }
        public DateTime UpdateDate { get; set; }

        public static SeminarRegisterSE ToSeminarRegisterSE(SeminarRegisterBo info, CustomerBo customer, List<ParentBo> parents, 
                                    List<StudyHistoryBo> studyHistories, List<StudyAbroadBo> studyAbroads, List<LanguageBo> languages)
        {
            var register = new SeminarRegisterSE();

            if (customer != null)
            {
                var parentInfo = "";
                var school = "";
                var className = "";
                var gpa = "";
                var level = "";
                var major = "";
                var abroadTime = "";
                var financial = "";
                var language = "";
                var source = 0;
                var sourceName = "";

                if(parents != null && parents.Any())
                {
                    foreach (var item in parents)
                    {
                        parentInfo += string.Format("Họ tên: {0}, điện thoại: {1}, ", item.Name, item.Phone);
                    }
                }

                if (studyHistories != null && studyHistories.Any())
                {
                    school = studyHistories[0].School;
                    gpa = studyHistories[0].Score;
                    className = studyHistories[0].Class;
                }

                if (studyAbroads != null && studyAbroads.Any())
                {
                    level = StudyLevel.Instant().GetValueByKey(studyAbroads[0].Level);
                    major = studyAbroads[0].Major;
                    abroadTime = studyAbroads[0].Time;
                }

                if (languages != null && languages.Any())
                {
                    foreach (var item in languages)
                    {
                        language += string.Format("Chứng chỉ: {0}, điểm: {1}, ", item.CertificateName, item.Score);
                    }
                }

                register = new SeminarRegisterSE()
                {
                    Id = info.Id,
                    SeminarId = info.SeminarId,
                    SeminarPlaceId = info.SeminarPlaceId,
                    CustomerId = customer.Id,
                    FullName = customer.Fullname,
                    Phone = customer.Phone,
                    Email = customer.Email,
                    Birthday = customer.Birthday.Value,
                    ParentInfo = parentInfo,
                    School = school,
                    Class = className,
                    GPA = gpa,
                    EducationLevel = level,
                    Major = major,
                    StudyAbroadTime = abroadTime,
                    Financial = financial,
                    LanguageInfo = language,
                    Source = source,
                    SourceName = sourceName,
                    TitleId = customer.EmployeeId,
                    TitleName = customer.EmployeeName,
                    UpdateDate = customer.UpdateDate.Value,
                };
            }

            return register;
        }

        public static SeminarRegisterExcel ToSeminarRegisterExcel(SeminarRegisterSE obj)
        {
            return new SeminarRegisterExcel()
            {
                CustomerId = obj.CustomerId,
                FullName = obj.FullName,
                Phone = obj.Phone,
                Email = obj.Email,
                Birthday = obj.Birthday.ToString("dd/MM/yyyy"),
                School = obj.School,
                Class = obj.Class,
                EducationLevel = obj.EducationLevel,
                Major = obj.Major,
                StudyAbroadTime = obj.StudyAbroadTime,
                Financial = obj.Financial,
                GPA = obj.GPA,
                LanguageInfo = obj.LanguageInfo,
                SourceName = obj.SourceName
            };
        }
    }
}
