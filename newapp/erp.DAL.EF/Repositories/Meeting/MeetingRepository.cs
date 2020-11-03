using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erp.Model;

namespace erp.DAL.EF
{
    public class MeetingRepository
    {
        public static List<Meeting> GetAll()
        {
            using (var m = Model.CreateModel())
            {
                return m.Meetings.ToList();
            }
        }

        public static void Create(Meeting m)
        {
            using (var model = Model.CreateModel())
            {
                if (m.Members != null)
                {
                    var memberIds = m.Members.Select(mb => mb.userid).ToList();
                    m.Members.Clear();
                    foreach (var mid in memberIds)
                    {
                        var user = model.Users.FirstOrDefault(u => u.userid == mid);
                        if (user != null)
                            m.Members.Add(user);
                    }    
                }
                
                m.DateCreated = DateTime.Now;
                model.Meetings.Add(m);
                model.SaveChanges();
            }
        }

        public static void Update(Meeting m)
        {
            using (var model = Model.CreateModel())
            {
                var old = model.Meetings.Include("Members").Include("Details").Include("Details.Responsibilities").FirstOrDefault(me => me.meeting_id == m.meeting_id);
                if (old != null)
                {
                    old.meetingDate = m.meetingDate;
                    old.description = m.description;
                    old.DateModified = DateTime.Now;
                    //Add existing
                    foreach (var mb in m.Members)
                    {
                        if (old.Members.FirstOrDefault(om => om.userid == mb.userid) == null)
                        {
                            var user = model.Users.FirstOrDefault(u => u.userid == mb.userid);
                            if(user != null)
                                old.Members.Add(user);
                        }
                    }

                    var deleted = old.Members.Where(mb => m.Members.Count(me => me.userid == mb.userid) == 0).ToList();
                    foreach (var d in deleted)
                    {
                        old.Members.Remove(d);
                    }

                    //Details
                    //Add existing
                    foreach (var de in m.Details)
                    {
                        Meeting_detail oldDe;
                        if ((oldDe = old.Details.FirstOrDefault(d => d.meeting_detail_id == de.meeting_detail_id)) == null)
                        {
                            old.Details.Add(de);
                        }
                        else
                        {
                            oldDe.issue = de.issue;
                            oldDe.resolution = de.resolution;
                            oldDe.status = de.status;
                            oldDe.heading = de.heading;
                            if (de.Responsibilities != null)
                            {
                                foreach (var r in de.Responsibilities)
                                {
                                    r.User = null;
                                    Meeting_detail_responsibility oldRe;
                                    if ((oldRe = oldDe.Responsibilities.FirstOrDefault(re => re.useruserid == r.useruserid)) == null)
                                        oldDe.Responsibilities.Add(r);
                                    else
                                    {
                                        oldRe.expected_resolution_date = r.expected_resolution_date;
                                    }
                                }        
                            }
                            model.Entry(oldDe).Collection("Images").Load();
                            if (de.Images != null)
                            {
                                foreach (var im in de.Images)
                                {
                                    Meeting_detail_image oldIm;
                                    if ((oldIm = oldDe.Images.FirstOrDefault(i => i.id == im.id)) == null)
                                        oldDe.Images.Add(im);
                                    else
                                    {
                                        oldIm.image = im.image;
                                    }
                                }
                                var deletedImages =
                                    oldDe.Images.Where(im => de.Images.Count(i => i.id == im.id) == 0).ToList();
                                foreach (var d in deletedImages)
                                {
                                    model.Entry(d).State = EntityState.Deleted;
                                }
                            }
                            
                        }
                        
                    }

                    var deletedDetails = old.Details.Where(de => m.Details.Count(d => d.meeting_detail_id == de.meeting_detail_id) == 0).ToList();
                    foreach (var d in deletedDetails)
                    {
                        //old.Details.Remove(d);
                        d.Responsibilities = null;  //avoid headache with foreign keys
                        d.Images = null;
                        model.Entry(d).State = EntityState.Deleted;
                    }

                    model.SaveChanges();
                }
            }
        }

        public static Meeting GetById(int id)
        {
            using (var model = Model.CreateModel())
            {
                var meeting = model.Meetings.Include("Members").Include("Details").Include("Details.Responsibilities").Include("Details.Responsibilities.User").FirstOrDefault(m => m.meeting_id == id);
                if (meeting != null && meeting.Details != null) {
                    foreach (var d in meeting.Details)
                    {
                        //d.Images = GetImages(model,d.meeting_detail_id);
                        model.Entry(d).Collection("Images").Load();
                    }
                }
                return meeting;
            }
        }

        //public static List<Meeting_detail_image> GetImages(Model m, int detail_id)
        //{
        //    return m.MeetingDetailImages.Where(im => im.meeting_detail_id == detail_id).ToList();
        //}

        public static void Delete(int id)
        {
            using (var m = Model.CreateModel())
            {
                m.Database.ExecuteSqlCommand("DELETE FROM meeting WHERE meeting_id=@p0", id);
            }
        }

        public static List<Meeting> Search(string text)
        {
            using (var model = Model.CreateModel())
            {
                return
                    model.Meetings.Where(m => m.Details.Any(d => d.issue.Contains(text) || d.resolution.Contains(text)))
                        .ToList();
            }
        }
    }
}
