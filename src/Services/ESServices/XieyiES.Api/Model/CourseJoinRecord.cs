namespace XieyiES.Api.Model
{
    public class CourseJoinRecord
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 用户编码
        /// </summary>
        public string UserCode { get; set; }

        /// <summary>
        /// 课程id
        /// </summary>
        public string CourseId { get; set; }

        /// <summary>
        /// 章节id
        /// </summary>
        public string ChapterId { get; set; }

        /// <summary>
        /// 加入课程的课程类别
        /// </summary>
        public string CourseType { get; set; }

        /// <summary>
        /// 学习状态
        /// </summary>
        public int LearningStatus { get; set; }  
    }
}
