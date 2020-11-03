using erp.DAL.EF.Repositories;
using erp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RefactorThis.GraphDiff;

namespace erp.DAL.EF
{
    public class TcDocumentRepository : GenericRepository<tc_document>
    {
        public TcDocumentRepository(Model context) : base(context)
        {
        }

        public override void Insert(tc_document document)
        {
            /*if(document.Users != null) {  
                document.Users = context.Users.Where(us => document.Users.Select(u => u.userid).Contains(us.userid)).ToList();
                    
            }
            base.Insert(document);*/
            context.UpdateGraph(document, map => map.AssociatedCollection(d => d.Users));
        }

        public override void Update(tc_document document)
        {
            //Update graph fails on company_id (required property)
            //context.UpdateGraph(document, map => map.AssociatedCollection(d => d.Users));

            //manual way
            var oldDoc = Get(d => d.id == document.id, includeProperties: "Users").FirstOrDefault();
            if(oldDoc != null) {
                context.Entry(oldDoc).CurrentValues.SetValues(document);
                var deletedUsers = oldDoc.Users.Where(oldUs => document.Users.Count(u => u.userid == oldUs.userid) == 0).ToList();
                var newUserIds = document.Users.Where(u => oldDoc.Users.Count(oldUs => oldUs.userid == u.userid) == 0).Select(u => u.userid).ToList();
                if(newUserIds.Count > 0)
                    oldDoc.Users.AddRange(context.Set<User>().Where(us=>newUserIds.Contains(us.userid)));
                foreach (var d in deletedUsers)
                    oldDoc.Users.Remove(d);
            }
            
        }
    }
}
