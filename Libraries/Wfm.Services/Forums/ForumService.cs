﻿using System;
using System.Collections.Generic;
using System.Linq;
using Wfm.Core;
using Wfm.Core.Caching;
using Wfm.Core.Data;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Forums;
using Wfm.Services.Common;
using Wfm.Services.Accounts;
using Wfm.Services.Events;
using Wfm.Services.Messages;

namespace Wfm.Services.Forums
{
    /// <summary>
    /// Forum service
    /// </summary>
    public partial class ForumService : IForumService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        private const string FORUMGROUP_ALL_KEY = "Wfm.forumgroup.all";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : forum group ID
        /// </remarks>
        private const string FORUM_ALLBYFORUMGROUPID_KEY = "Wfm.forum.allbyforumgroupid-{0}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string FORUMGROUP_PATTERN_KEY = "Wfm.forumgroup.";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string FORUM_PATTERN_KEY = "Wfm.forum.";

        #endregion

        #region Fields
        private readonly IRepository<ForumGroup> _forumGroupRepository;
        private readonly IRepository<Forum> _forumRepository;
        private readonly IRepository<ForumTopic> _forumTopicRepository;
        private readonly IRepository<ForumPost> _forumPostRepository;
        private readonly IRepository<PrivateMessage> _forumPrivateMessageRepository;
        private readonly IRepository<ForumSubscription> _forumSubscriptionRepository;
        private readonly ForumSettings _forumSettings;
        private readonly IRepository<Account> _accountRepository;
        private readonly ICacheManager _cacheManager;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IAccountService _accountService;
        private readonly IWorkContext _workContext;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="forumGroupRepository">Forum group repository</param>
        /// <param name="forumRepository">Forum repository</param>
        /// <param name="forumTopicRepository">Forum topic repository</param>
        /// <param name="forumPostRepository">Forum post repository</param>
        /// <param name="forumPrivateMessageRepository">Private message repository</param>
        /// <param name="forumSubscriptionRepository">Forum subscription repository</param>
        /// <param name="forumSettings">Forum settings</param>
        /// <param name="accountRepository">Account repository</param>
        /// <param name="genericAttributeService">Generic attribute service</param>
        /// <param name="accountService">Account service</param>
        /// <param name="workContext">Work context</param>
        /// <param name="workflowMessageService">Workflow message service</param>
        /// <param name="eventPublisher">Event published</param>
        public ForumService(ICacheManager cacheManager,
            IRepository<ForumGroup> forumGroupRepository,
            IRepository<Forum> forumRepository,
            IRepository<ForumTopic> forumTopicRepository,
            IRepository<ForumPost> forumPostRepository,
            IRepository<PrivateMessage> forumPrivateMessageRepository,
            IRepository<ForumSubscription> forumSubscriptionRepository,
            ForumSettings forumSettings,
            IRepository<Account> accountRepository,
            IGenericAttributeService genericAttributeService,
            IAccountService accountService,
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService,
            IEventPublisher eventPublisher
            )
        {
            this._cacheManager = cacheManager;
            this._forumGroupRepository = forumGroupRepository;
            this._forumRepository = forumRepository;
            this._forumTopicRepository = forumTopicRepository;
            this._forumPostRepository = forumPostRepository;
            this._forumPrivateMessageRepository = forumPrivateMessageRepository;
            this._forumSubscriptionRepository = forumSubscriptionRepository;
            this._forumSettings = forumSettings;
            this._accountRepository = accountRepository;
            this._genericAttributeService = genericAttributeService;
            this._accountService = accountService;
            this._workContext = workContext;
            this._workflowMessageService = workflowMessageService;
            _eventPublisher = eventPublisher;
        }
        #endregion

        #region Utilities
        
        /// <summary>
        /// Update forum stats
        /// </summary>
        /// <param name="forumId">The forum identifier</param>
        private void UpdateForumStats(int forumId)
        {
            if (forumId == 0)
            {
                return;
            }
            var forum = GetForumById(forumId);
            if (forum == null)
            {
                return;
            }

            //number of topics
            var queryNumTopics = from ft in _forumTopicRepository.Table
                                 where ft.ForumId == forumId
                                 select ft.Id;
            int numTopics = queryNumTopics.Count();

            //number of posts
            var queryNumPosts = from ft in _forumTopicRepository.Table
                                join fp in _forumPostRepository.Table on ft.Id equals fp.TopicId
                                where ft.ForumId == forumId
                                select fp.Id;
            int numPosts = queryNumPosts.Count();

            //last values
            int lastTopicId = 0;
            int lastPostId = 0;
            int lastPostAccountId = 0;
            DateTime? lastPostTime = null;
            var queryLastValues = from ft in _forumTopicRepository.Table
                                  join fp in _forumPostRepository.Table on ft.Id equals fp.TopicId
                                  where ft.ForumId == forumId
                                  orderby fp.CreatedOnUtc descending, ft.CreatedOnUtc descending
                                  select new
                                  {
                                      LastTopicId = ft.Id,
                                      LastPostId = fp.Id,
                                      LastPostAccountId = fp.AccountId,
                                      LastPostTime = fp.CreatedOnUtc
                                  };
            var lastValues = queryLastValues.FirstOrDefault();
            if (lastValues != null)
            {
                lastTopicId = lastValues.LastTopicId;
                lastPostId = lastValues.LastPostId;
                lastPostAccountId = lastValues.LastPostAccountId;
                lastPostTime = lastValues.LastPostTime;
            }

            //update forum
            forum.NumTopics = numTopics;
            forum.NumPosts = numPosts;
            forum.LastTopicId = lastTopicId;
            forum.LastPostId = lastPostId;
            forum.LastPostAccountId = lastPostAccountId;
            forum.LastPostTime = lastPostTime;
            UpdateForum(forum);
        }

        /// <summary>
        /// Update forum topic stats
        /// </summary>
        /// <param name="forumTopicId">The forum topic identifier</param>
        private void UpdateForumTopicStats(int forumTopicId)
        {
            if (forumTopicId == 0)
            {
                return;
            }
            var forumTopic = GetTopicById(forumTopicId);
            if (forumTopic == null)
            {
                return;
            }

            //number of posts
            var queryNumPosts = from fp in _forumPostRepository.Table
                                where fp.TopicId == forumTopicId
                                select fp.Id;
            int numPosts = queryNumPosts.Count();

            //last values
            int lastPostId = 0;
            int lastPostAccountId = 0;
            DateTime? lastPostTime = null;
            var queryLastValues = from fp in _forumPostRepository.Table
                                  where fp.TopicId == forumTopicId
                                  orderby fp.CreatedOnUtc descending
                                  select new
                                  {
                                      LastPostId = fp.Id,
                                      LastPostAccountId = fp.AccountId,
                                      LastPostTime = fp.CreatedOnUtc
                                  };
            var lastValues = queryLastValues.FirstOrDefault();
            if (lastValues != null)
            {
                lastPostId = lastValues.LastPostId;
                lastPostAccountId = lastValues.LastPostAccountId;
                lastPostTime = lastValues.LastPostTime;
            }

            //update topic
            forumTopic.NumPosts = numPosts;
            forumTopic.LastPostId = lastPostId;
            forumTopic.LastPostAccountId = lastPostAccountId;
            forumTopic.LastPostTime = lastPostTime;
            UpdateTopic(forumTopic);
        }

        /// <summary>
        /// Update account stats
        /// </summary>
        /// <param name="accountId">The account identifier</param>
        private void UpdateAccountStats(int accountId)
        {
            if (accountId == 0)
            {
                return;
            }

            var account = _accountService.GetAccountById(accountId);

            if (account == null)
            {
                return;
            }

            var query = from fp in _forumPostRepository.Table
                        where fp.AccountId == accountId
                        select fp.Id;
            int numPosts = query.Count();

            _genericAttributeService.SaveAttribute(account, SystemAccountAttributeNames.ForumPostCount, numPosts);
        }

        private bool IsForumModerator(Account account)
        {
            if (account.IsForumModerator())
            {
                return true;
            }
            return false;
        }

        private bool IsAccountAuthorizedForOperation(Account account, int itemUserId, string operation)
        {
            if (account == null || account.IsGuest())
            {
                return false;
            }

            if (IsForumModerator(account))
            {
                return true;
            }

            bool result = false;
            switch (operation)
            {
                case "DeleteTopic":
                case "DeletePosts":
                    result = (_forumSettings.AllowAccountsToDeletePosts && account.Id == itemUserId);
                    break;
                case "EditTopic":
                case "EditPosts":
                    result = (_forumSettings.AllowAccountsToEditPosts && account.Id == itemUserId);
                    break;
            }
            
            return result;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a forum group
        /// </summary>
        /// <param name="forumGroup">Forum group</param>
        public virtual void DeleteForumGroup(ForumGroup forumGroup)
        {
            if (forumGroup == null)
            {
                throw new ArgumentNullException("forumGroup");
            }

            _forumGroupRepository.Delete(forumGroup);

            _cacheManager.RemoveByPattern(FORUMGROUP_PATTERN_KEY);
            _cacheManager.RemoveByPattern(FORUM_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(forumGroup);
        }

        /// <summary>
        /// Gets a forum group
        /// </summary>
        /// <param name="forumGroupId">The forum group identifier</param>
        /// <returns>Forum group</returns>
        public virtual ForumGroup GetForumGroupById(int forumGroupId)
        {
            if (forumGroupId == 0)
            {
                return null;
            }

            return _forumGroupRepository.GetById(forumGroupId);
        }

        /// <summary>
        /// Gets all forum groups
        /// </summary>
        /// <returns>Forum groups</returns>
        public virtual IList<ForumGroup> GetAllForumGroups()
        {
            string key = string.Format(FORUMGROUP_ALL_KEY);
            return _cacheManager.Get(key, () =>
            {
                var query = from fg in _forumGroupRepository.Table
                            orderby fg.DisplayOrder
                            select fg;
                return query.ToList();
            });
        }

        /// <summary>
        /// Inserts a forum group
        /// </summary>
        /// <param name="forumGroup">Forum group</param>
        public virtual void InsertForumGroup(ForumGroup forumGroup)
        {
            if (forumGroup == null)
            {
                throw new ArgumentNullException("forumGroup");
            }

            _forumGroupRepository.Insert(forumGroup);

            //cache
            _cacheManager.RemoveByPattern(FORUMGROUP_PATTERN_KEY);
            _cacheManager.RemoveByPattern(FORUM_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(forumGroup);
        }

        /// <summary>
        /// Updates the forum group
        /// </summary>
        /// <param name="forumGroup">Forum group</param>
        public virtual void UpdateForumGroup(ForumGroup forumGroup)
        {
            if (forumGroup == null)
            {
                throw new ArgumentNullException("forumGroup");
            }

            _forumGroupRepository.Update(forumGroup);

            //cache
            _cacheManager.RemoveByPattern(FORUMGROUP_PATTERN_KEY);
            _cacheManager.RemoveByPattern(FORUM_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(forumGroup);
        }

        /// <summary>
        /// Deletes a forum
        /// </summary>
        /// <param name="forum">Forum</param>
        public virtual void DeleteForum(Forum forum)
        {            
            if (forum == null)
            {
                throw new ArgumentNullException("forum");
            }

            //delete forum subscriptions (topics)
            var queryTopicIds = from ft in _forumTopicRepository.Table
                           where ft.ForumId == forum.Id
                           select ft.Id;
            var queryFs1 = from fs in _forumSubscriptionRepository.Table
                           where queryTopicIds.Contains(fs.TopicId)
                           select fs;
            foreach (var fs in queryFs1.ToList())
            {
                _forumSubscriptionRepository.Delete(fs);
                //event notification
                _eventPublisher.EntityDeleted(fs);
            }

            //delete forum subscriptions (forum)
            var queryFs2 = from fs in _forumSubscriptionRepository.Table
                           where fs.ForumId == forum.Id
                           select fs;
            foreach (var fs2 in queryFs2.ToList())
            {
                _forumSubscriptionRepository.Delete(fs2);
                //event notification
                _eventPublisher.EntityDeleted(fs2);
            }

            //delete forum
            _forumRepository.Delete(forum);

            _cacheManager.RemoveByPattern(FORUMGROUP_PATTERN_KEY);
            _cacheManager.RemoveByPattern(FORUM_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(forum);
        }

        /// <summary>
        /// Gets a forum
        /// </summary>
        /// <param name="forumId">The forum identifier</param>
        /// <returns>Forum</returns>
        public virtual Forum GetForumById(int forumId)
        {
            if (forumId == 0)
                return null;

            return _forumRepository.GetById(forumId);
        }

        /// <summary>
        /// Gets forums by forum group identifier
        /// </summary>
        /// <param name="forumGroupId">The forum group identifier</param>
        /// <returns>Forums</returns>
        public virtual IList<Forum> GetAllForumsByGroupId(int forumGroupId)
        {
            string key = string.Format(FORUM_ALLBYFORUMGROUPID_KEY, forumGroupId);
            return _cacheManager.Get(key, () =>
            {
                var query = from f in _forumRepository.Table
                            orderby f.DisplayOrder
                            where f.ForumGroupId == forumGroupId
                            select f;
                var forums = query.ToList();
                return forums;
            });
        }

        /// <summary>
        /// Inserts a forum
        /// </summary>
        /// <param name="forum">Forum</param>
        public virtual void InsertForum(Forum forum)
        {
            if (forum == null)
            {
                throw new ArgumentNullException("forum");
            }

            _forumRepository.Insert(forum);

            _cacheManager.RemoveByPattern(FORUMGROUP_PATTERN_KEY);
            _cacheManager.RemoveByPattern(FORUM_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(forum);
        }

        /// <summary>
        /// Updates the forum
        /// </summary>
        /// <param name="forum">Forum</param>
        public virtual void UpdateForum(Forum forum)
        {
            if (forum == null)
            {
                throw new ArgumentNullException("forum");
            }

            _forumRepository.Update(forum);
            
            _cacheManager.RemoveByPattern(FORUMGROUP_PATTERN_KEY);
            _cacheManager.RemoveByPattern(FORUM_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(forum);
        }

        /// <summary>
        /// Deletes a forum topic
        /// </summary>
        /// <param name="forumTopic">Forum topic</param>
        public virtual void DeleteTopic(ForumTopic forumTopic)
        {            
            if (forumTopic == null)
            {                
                throw new ArgumentNullException("forumTopic");
            }                

            int accountId = forumTopic.AccountId;
            int forumId = forumTopic.ForumId;

            //delete topic
            _forumTopicRepository.Delete(forumTopic);

            //delete forum subscriptions
            var queryFs = from ft in _forumSubscriptionRepository.Table
                          where ft.TopicId == forumTopic.Id
                          select ft;
            var forumSubscriptions = queryFs.ToList();
            foreach (var fs in forumSubscriptions)
            {
                _forumSubscriptionRepository.Delete(fs);
                //event notification
                _eventPublisher.EntityDeleted(fs);
            }

            //update stats
            UpdateForumStats(forumId);
            UpdateAccountStats(accountId);

            _cacheManager.RemoveByPattern(FORUMGROUP_PATTERN_KEY);
            _cacheManager.RemoveByPattern(FORUM_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(forumTopic);
        }

        /// <summary>
        /// Gets a forum topic
        /// </summary>
        /// <param name="forumTopicId">The forum topic identifier</param>
        /// <returns>Forum Topic</returns>
        public virtual ForumTopic GetTopicById(int forumTopicId)
        {
            return GetTopicById(forumTopicId, false);
        }

        /// <summary>
        /// Gets a forum topic
        /// </summary>
        /// <param name="forumTopicId">The forum topic identifier</param>
        /// <param name="increaseViews">The value indicating whether to increase forum topic views</param>
        /// <returns>Forum Topic</returns>
        public virtual ForumTopic GetTopicById(int forumTopicId, bool increaseViews)
        {
            if (forumTopicId == 0)
                return null;

            var forumTopic = _forumTopicRepository.GetById(forumTopicId);
            if (forumTopic == null)
                return null;

            if (increaseViews)
            {
                forumTopic.Views = ++forumTopic.Views;
                UpdateTopic(forumTopic);
            }

            return forumTopic;
        }

        /// <summary>
        /// Gets all forum topics
        /// </summary>
        /// <param name="forumId">The forum identifier</param>
        /// <param name="accountId">The account identifier</param>
        /// <param name="keywords">Keywords</param>
        /// <param name="searchType">Search type</param>
        /// <param name="limitDays">Limit by the last number days; 0 to load all topics</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Forum Topics</returns>
        public virtual IPagedList<ForumTopic> GetAllTopics(int forumId,
            int accountId, string keywords, ForumSearchType searchType,
            int limitDays, int pageIndex, int pageSize)
        {
            DateTime? limitDate = null;
            if (limitDays > 0)
            {
                limitDate = DateTime.UtcNow.AddDays(-limitDays);
            }
            //we need to cast it to int, otherwise it won't work in SQLCE4
            //we cannot use String.IsNullOrEmpty in query because it causes SqlCeException on SQLCE4
            bool searchKeywords = !String.IsNullOrEmpty(keywords);
            bool searchTopicTitles = searchType == ForumSearchType.All || searchType == ForumSearchType.TopicTitlesOnly;
            bool searchPostText = searchType == ForumSearchType.All || searchType == ForumSearchType.PostTextOnly;
            var query1 = from ft in _forumTopicRepository.Table
                         join fp in _forumPostRepository.Table on ft.Id equals fp.TopicId
                         where
                         (forumId == 0 || ft.ForumId == forumId) &&
                         (accountId == 0 || ft.AccountId == accountId) &&
                         (
                            !searchKeywords ||
                            (searchTopicTitles && ft.Subject.Contains(keywords)) ||
                            (searchPostText && fp.Text.Contains(keywords))) && 
                         (!limitDate.HasValue || limitDate.Value <= ft.LastPostTime)
                         select ft.Id;

            var query2 = from ft in _forumTopicRepository.Table
                         where query1.Contains(ft.Id)
                         orderby ft.TopicTypeId descending, ft.LastPostTime descending, ft.Id descending
                         select ft;

            var topics = new PagedList<ForumTopic>(query2, pageIndex, pageSize);
            return topics;
        }

        /// <summary>
        /// Gets active forum topics
        /// </summary>
        /// <param name="forumId">The forum identifier</param>
        /// <param name="count">Count of forum topics to return</param>
        /// <returns>Forum Topics</returns>
        public virtual IList<ForumTopic> GetActiveTopics(int forumId, int count)
        {
            var query1 = from ft in _forumTopicRepository.Table
                         where
                         (forumId == 0 || ft.ForumId == forumId) &&
                         (ft.LastPostTime.HasValue)
                         select ft.Id;

            var query2 = from ft in _forumTopicRepository.Table
                         where query1.Contains(ft.Id)
                         orderby ft.LastPostTime descending
                         select ft;

            var forumTopics = query2.Take(count).ToList();
            return forumTopics;
        }

        /// <summary>
        /// Inserts a forum topic
        /// </summary>
        /// <param name="forumTopic">Forum topic</param>
        /// <param name="sendNotifications">A value indicating whether to send notifications to subscribed accounts</param>
        public virtual void InsertTopic(ForumTopic forumTopic, bool sendNotifications)
        {
            if (forumTopic == null)
            {
                throw new ArgumentNullException("forumTopic");
            }

            _forumTopicRepository.Insert(forumTopic);

            //update stats
            UpdateForumStats(forumTopic.ForumId);

            //cache            
            _cacheManager.RemoveByPattern(FORUMGROUP_PATTERN_KEY);
            _cacheManager.RemoveByPattern(FORUM_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(forumTopic);

            //send notifications
            if (sendNotifications)
            {
                var forum = forumTopic.Forum;
                var subscriptions = GetAllSubscriptions(0, forum.Id, 0, 0, int.MaxValue);
                var languageId = _workContext.WorkingLanguage.Id;

                foreach (var subscription in subscriptions)
                {
                    if (subscription.AccountId == forumTopic.AccountId)
                    {
                        continue;
                    }

                    if (!String.IsNullOrEmpty(subscription.Account.Email))
                    {
                        _workflowMessageService.SendNewForumTopicMessage(subscription.Account, forumTopic,
                            forum, languageId);
                    }
                }
            }
        }

        /// <summary>
        /// Updates the forum topic
        /// </summary>
        /// <param name="forumTopic">Forum topic</param>
        public virtual void UpdateTopic(ForumTopic forumTopic)
        {
            if (forumTopic == null)
            {
                throw new ArgumentNullException("forumTopic");
            }

            _forumTopicRepository.Update(forumTopic);

            _cacheManager.RemoveByPattern(FORUMGROUP_PATTERN_KEY);
            _cacheManager.RemoveByPattern(FORUM_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(forumTopic);
        }

        /// <summary>
        /// Moves the forum topic
        /// </summary>
        /// <param name="forumTopicId">The forum topic identifier</param>
        /// <param name="newForumId">New forum identifier</param>
        /// <returns>Moved forum topic</returns>
        public virtual ForumTopic MoveTopic(int forumTopicId, int newForumId)
        {
            var forumTopic = GetTopicById(forumTopicId);
            if (forumTopic == null)
                return forumTopic;

            if (this.IsAccountAllowedToMoveTopic(_workContext.CurrentAccount, forumTopic))
            {
                int previousForumId = forumTopic.ForumId;
                var newForum = GetForumById(newForumId);

                if (newForum != null)
                {
                    if (previousForumId != newForumId)
                    {
                        forumTopic.ForumId = newForum.Id;
                        forumTopic.UpdatedOnUtc = DateTime.UtcNow;
                        UpdateTopic(forumTopic);

                        //update forum stats
                        UpdateForumStats(previousForumId);
                        UpdateForumStats(newForumId);
                    }
                }
            }
            return forumTopic;
        }

        /// <summary>
        /// Deletes a forum post
        /// </summary>
        /// <param name="forumPost">Forum post</param>
        public virtual void DeletePost(ForumPost forumPost)
        {            
            if (forumPost == null)
            {
                throw new ArgumentNullException("forumPost");
            }

            int forumTopicId = forumPost.TopicId;
            int accountId = forumPost.AccountId;
            var forumTopic = this.GetTopicById(forumTopicId);
            int forumId = forumTopic.ForumId;

            //delete topic if it was the first post
            bool deleteTopic = false;
            ForumPost firstPost = forumTopic.GetFirstPost(this);
            if (firstPost != null && firstPost.Id == forumPost.Id)
            {
                deleteTopic = true;
            }

            //delete forum post
            _forumPostRepository.Delete(forumPost);

            //delete topic
            if (deleteTopic)
            {
                DeleteTopic(forumTopic);
            }

            //update stats
            if (!deleteTopic)
            {
                UpdateForumTopicStats(forumTopicId);
            }
            UpdateForumStats(forumId);
            UpdateAccountStats(accountId);

            //clear cache            
            _cacheManager.RemoveByPattern(FORUMGROUP_PATTERN_KEY);
            _cacheManager.RemoveByPattern(FORUM_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(forumPost);

        }

        /// <summary>
        /// Gets a forum post
        /// </summary>
        /// <param name="forumPostId">The forum post identifier</param>
        /// <returns>Forum Post</returns>
        public virtual ForumPost GetPostById(int forumPostId)
        {
            if (forumPostId == 0)
                return null;

            return _forumPostRepository.GetById(forumPostId);
        }

        /// <summary>
        /// Gets all forum posts
        /// </summary>
        /// <param name="forumTopicId">The forum topic identifier</param>
        /// <param name="accountId">The account identifier</param>
        /// <param name="keywords">Keywords</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Posts</returns>
        public virtual IPagedList<ForumPost> GetAllPosts(int forumTopicId,
            int accountId, string keywords, int pageIndex, int pageSize)
        {
            return GetAllPosts(forumTopicId, accountId, keywords, true,
                pageIndex, pageSize);
        }

        /// <summary>
        /// Gets all forum posts
        /// </summary>
        /// <param name="forumTopicId">The forum topic identifier</param>
        /// <param name="accountId">The account identifier</param>
        /// <param name="keywords">Keywords</param>
        /// <param name="ascSort">Sort order</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Forum Posts</returns>
        public virtual IPagedList<ForumPost> GetAllPosts(int forumTopicId, int accountId,
            string keywords, bool ascSort, int pageIndex, int pageSize)
        {
            var query = _forumPostRepository.Table;
            if (forumTopicId > 0)
            {
                query = query.Where(fp => forumTopicId == fp.TopicId);
            }
            if (accountId > 0)
            {
                query = query.Where(fp => accountId == fp.AccountId);
            }
            if (!String.IsNullOrEmpty(keywords))
            {
                query = query.Where(fp => fp.Text.Contains(keywords));
            }
            if (ascSort)
            {
                query = query.OrderBy(fp => fp.CreatedOnUtc).ThenBy(fp => fp.Id);
            }
            else
            {
                query = query.OrderByDescending(fp => fp.CreatedOnUtc).ThenBy(fp => fp.Id);
            }

            var forumPosts = new PagedList<ForumPost>(query, pageIndex, pageSize);

            return forumPosts;
        }

        /// <summary>
        /// Inserts a forum post
        /// </summary>
        /// <param name="forumPost">The forum post</param>
        /// <param name="sendNotifications">A value indicating whether to send notifications to subscribed accounts</param>
        public virtual void InsertPost(ForumPost forumPost, bool sendNotifications)
        {
            if (forumPost == null)
            {
                throw new ArgumentNullException("forumPost");
            }

            _forumPostRepository.Insert(forumPost);

            //update stats
            int accountId = forumPost.AccountId;
            var forumTopic = this.GetTopicById(forumPost.TopicId);
            int forumId = forumTopic.ForumId;
            UpdateForumTopicStats(forumPost.TopicId);
            UpdateForumStats(forumId);
            UpdateAccountStats(accountId);

            //clear cache            
            _cacheManager.RemoveByPattern(FORUMGROUP_PATTERN_KEY);
            _cacheManager.RemoveByPattern(FORUM_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(forumPost);

            //notifications
            if (sendNotifications)
            {
                var forum = forumTopic.Forum;
                var subscriptions = GetAllSubscriptions(0, 0,
                    forumTopic.Id, 0, int.MaxValue);

                var languageId = _workContext.WorkingLanguage.Id;

                int friendlyTopicPageIndex = CalculateTopicPageIndex(forumPost.TopicId,
                    _forumSettings.PostsPageSize > 0 ? _forumSettings.PostsPageSize : 10, 
                    forumPost.Id) + 1;

                foreach (ForumSubscription subscription in subscriptions)
                {
                    if (subscription.AccountId == forumPost.AccountId)
                    {
                        continue;
                    }

                    if (!String.IsNullOrEmpty(subscription.Account.Email))
                    {
                        _workflowMessageService.SendNewForumPostMessage(subscription.Account, forumPost,
                            forumTopic, forum, friendlyTopicPageIndex, languageId);
                    }
                }
            }
        }

        /// <summary>
        /// Updates the forum post
        /// </summary>
        /// <param name="forumPost">Forum post</param>
        public virtual void UpdatePost(ForumPost forumPost)
        {
            //validation
            if (forumPost == null)
            {
                throw new ArgumentNullException("forumPost");
            }

            _forumPostRepository.Update(forumPost);

            _cacheManager.RemoveByPattern(FORUMGROUP_PATTERN_KEY);
            _cacheManager.RemoveByPattern(FORUM_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(forumPost);
        }

        /// <summary>
        /// Deletes a private message
        /// </summary>
        /// <param name="privateMessage">Private message</param>
        public virtual void DeletePrivateMessage(PrivateMessage privateMessage)
        {
            if (privateMessage == null)
            {
                throw new ArgumentNullException("privateMessage");
            }

            _forumPrivateMessageRepository.Delete(privateMessage);

            //event notification
            _eventPublisher.EntityDeleted(privateMessage);
        }

        /// <summary>
        /// Gets a private message
        /// </summary>
        /// <param name="privateMessageId">The private message identifier</param>
        /// <returns>Private message</returns>
        public virtual PrivateMessage GetPrivateMessageById(int privateMessageId)
        {
            if (privateMessageId == 0)
                return null;

            return _forumPrivateMessageRepository.GetById(privateMessageId);
        }

        /// <summary>
        /// Gets private messages
        /// </summary>
        /// <param name="franchiseId">The franchise identifier; pass 0 to load all messages</param>
        /// <param name="fromAccountId">The account identifier who sent the message</param>
        /// <param name="toAccountId">The account identifier who should receive the message</param>
        /// <param name="isRead">A value indicating whether loaded messages are read. false - to load not read messages only, 1 to load read messages only, null to load all messages</param>
        /// <param name="isDeletedByAuthor">A value indicating whether loaded messages are deleted by author. false - messages are not deleted by author, null to load all messages</param>
        /// <param name="isDeletedByRecipient">A value indicating whether loaded messages are deleted by recipient. false - messages are not deleted by recipient, null to load all messages</param>
        /// <param name="keywords">Keywords</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Private messages</returns>
        public virtual IPagedList<PrivateMessage> GetAllPrivateMessages(int franchiseId, int fromAccountId,
            int toAccountId, bool? isRead, bool? isDeletedByAuthor, bool? isDeletedByRecipient,
            string keywords, int pageIndex, int pageSize)
        {
            var query = _forumPrivateMessageRepository.Table;
            if (franchiseId > 0)
                query = query.Where(pm => franchiseId == pm.FranchiseId);
            if (fromAccountId > 0)
                query = query.Where(pm => fromAccountId == pm.FromAccountId);
            if (toAccountId > 0)
                query = query.Where(pm => toAccountId == pm.ToAccountId);
            if (isRead.HasValue)
                query = query.Where(pm => isRead.Value == pm.IsRead);
            if (isDeletedByAuthor.HasValue)
                query = query.Where(pm => isDeletedByAuthor.Value == pm.IsDeletedByAuthor);
            if (isDeletedByRecipient.HasValue)
                query = query.Where(pm => isDeletedByRecipient.Value == pm.IsDeletedByRecipient);
            if (!String.IsNullOrEmpty(keywords))
            {
                query = query.Where(pm => pm.Subject.Contains(keywords));
                query = query.Where(pm => pm.Text.Contains(keywords));
            }
            query = query.OrderByDescending(pm => pm.CreatedOnUtc);

            var privateMessages = new PagedList<PrivateMessage>(query, pageIndex, pageSize);

            return privateMessages;
        }

        /// <summary>
        /// Inserts a private message
        /// </summary>
        /// <param name="privateMessage">Private message</param>
        public virtual void InsertPrivateMessage(PrivateMessage privateMessage)
        {
            if (privateMessage == null)
            {
                throw new ArgumentNullException("privateMessage");
            }

            _forumPrivateMessageRepository.Insert(privateMessage);

            //event notification
            _eventPublisher.EntityInserted(privateMessage);

            var accountTo = _accountService.GetAccountById(privateMessage.ToAccountId);
            if (accountTo == null)
            {
                throw new WfmException("Recipient could not be loaded");
            }

            //UI notification
            _genericAttributeService.SaveAttribute(accountTo, SystemAccountAttributeNames.NotifiedAboutNewPrivateMessages, false, privateMessage.FranchiseId);

            //Email notification
        }

        /// <summary>
        /// Updates the private message
        /// </summary>
        /// <param name="privateMessage">Private message</param>
        public virtual void UpdatePrivateMessage(PrivateMessage privateMessage)
        {
            if (privateMessage == null)
                throw new ArgumentNullException("privateMessage");

            if (privateMessage.IsDeletedByAuthor && privateMessage.IsDeletedByRecipient)
            {
                _forumPrivateMessageRepository.Delete(privateMessage);
                //event notification
                _eventPublisher.EntityDeleted(privateMessage);
            }
            else
            {
                _forumPrivateMessageRepository.Update(privateMessage);
                //event notification
                _eventPublisher.EntityUpdated(privateMessage);
            }
        }

        /// <summary>
        /// Deletes a forum subscription
        /// </summary>
        /// <param name="forumSubscription">Forum subscription</param>
        public virtual void DeleteSubscription(ForumSubscription forumSubscription)
        {
            if (forumSubscription == null)
            {
                throw new ArgumentNullException("forumSubscription");
            }

            _forumSubscriptionRepository.Delete(forumSubscription);

            //event notification
            _eventPublisher.EntityDeleted(forumSubscription);
        }

        /// <summary>
        /// Gets a forum subscription
        /// </summary>
        /// <param name="forumSubscriptionId">The forum subscription identifier</param>
        /// <returns>Forum subscription</returns>
        public virtual ForumSubscription GetSubscriptionById(int forumSubscriptionId)
        {
            if (forumSubscriptionId == 0)
                return null;

            return _forumSubscriptionRepository.GetById(forumSubscriptionId);
        }

        /// <summary>
        /// Gets forum subscriptions
        /// </summary>
        /// <param name="accountId">The account identifier</param>
        /// <param name="forumId">The forum identifier</param>
        /// <param name="topicId">The topic identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Forum subscriptions</returns>
        public virtual IPagedList<ForumSubscription> GetAllSubscriptions(int accountId, int forumId,
            int topicId, int pageIndex, int pageSize)
        {
            var fsQuery = from fs in _forumSubscriptionRepository.Table
                          join c in _accountRepository.Table on fs.AccountId equals c.Id
                          where
                          (accountId == 0 || fs.AccountId == accountId) &&
                          (forumId == 0 || fs.ForumId == forumId) &&
                          (topicId == 0 || fs.TopicId == topicId) &&
                          (c.IsActive && !c.IsDeleted)
                          select fs.SubscriptionGuid;

            var query = from fs in _forumSubscriptionRepository.Table
                        where fsQuery.Contains(fs.SubscriptionGuid)
                        orderby fs.CreatedOnUtc descending, fs.SubscriptionGuid descending
                        select fs;

            var forumSubscriptions = new PagedList<ForumSubscription>(query, pageIndex, pageSize);
            return forumSubscriptions;
        }

        /// <summary>
        /// Inserts a forum subscription
        /// </summary>
        /// <param name="forumSubscription">Forum subscription</param>
        public virtual void InsertSubscription(ForumSubscription forumSubscription)
        {
            if (forumSubscription == null)
            {
                throw new ArgumentNullException("forumSubscription");
            }

            _forumSubscriptionRepository.Insert(forumSubscription);

            //event notification
            _eventPublisher.EntityInserted(forumSubscription);
        }

        /// <summary>
        /// Updates the forum subscription
        /// </summary>
        /// <param name="forumSubscription">Forum subscription</param>
        public virtual void UpdateSubscription(ForumSubscription forumSubscription)
        {
            if (forumSubscription == null)
            {
                throw new ArgumentNullException("forumSubscription");
            }
            
            _forumSubscriptionRepository.Update(forumSubscription);

            //event notification
            _eventPublisher.EntityUpdated(forumSubscription);
        }


        /// <summary>
        /// Check whether account is allowed to create new topics
        /// </summary>
        /// <param name="account">Account</param>
        /// <param name="forum">Forum</param>
        /// <returns>True if allowed, otherwise false</returns>
        public virtual bool IsAccountAllowedToCreateTopic(Account account, Forum forum)
        {
            if (forum == null)
            {
                return false;
            }

            if (account == null)
            {
                return false;
            }

            if (account.IsGuest() && !_forumSettings.AllowGuestsToCreateTopics)
            {
                return false;
            }

            if (IsForumModerator(account))
            {
                return true;
            }

            return true;
        }

        /// <summary>
        /// Check whether account is allowed to edit topic
        /// </summary>
        /// <param name="account">Account</param>
        /// <param name="topic">Topic</param>
        /// <returns>True if allowed, otherwise false</returns>
        public virtual bool IsAccountAllowedToEditTopic(Account account, ForumTopic topic)
        {
            if (topic == null)
                return false;

            return IsAccountAuthorizedForOperation(account, topic.AccountId, "EditTopic");
        }

        /// <summary>
        /// Check whether account is allowed to move topic
        /// </summary>
        /// <param name="account">Account</param>
        /// <param name="topic">Topic</param>
        /// <returns>True if allowed, otherwise false</returns>
        public virtual bool IsAccountAllowedToMoveTopic(Account account, ForumTopic topic)
        {
            if (topic == null)
            {
                return false;
            }

            if (account == null)
            {
                return false;
            }

            if (account.IsGuest())
            {
                return false;
            }

            if (IsForumModerator(account))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Check whether account is allowed to delete topic
        /// </summary>
        /// <param name="account">Account</param>
        /// <param name="topic">Topic</param>
        /// <returns>True if allowed, otherwise false</returns>
        public virtual bool IsAccountAllowedToDeleteTopic(Account account, ForumTopic topic)
        {
            if (topic == null)
                return false;

            return IsAccountAuthorizedForOperation(account, topic.AccountId, "DeleteTopic");
        }

        /// <summary>
        /// Check whether account is allowed to create new post
        /// </summary>
        /// <param name="account">Account</param>
        /// <param name="topic">Topic</param>
        /// <returns>True if allowed, otherwise false</returns>
        public virtual bool IsAccountAllowedToCreatePost(Account account, ForumTopic topic)
        {
            if (topic == null)
            {
                return false;
            }

            if (account == null)
            {
                return false;
            }

            if (account.IsGuest() && !_forumSettings.AllowGuestsToCreatePosts)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Check whether account is allowed to edit post
        /// </summary>
        /// <param name="account">Account</param>
        /// <param name="post">Topic</param>
        /// <returns>True if allowed, otherwise false</returns>
        public virtual bool IsAccountAllowedToEditPost(Account account, ForumPost post)
        {
            if (post == null)
                return false;

            return IsAccountAuthorizedForOperation(account, post.AccountId, "EditPosts");
        }
        
        /// <summary>
        /// Check whether account is allowed to delete post
        /// </summary>
        /// <param name="account">Account</param>
        /// <param name="post">Topic</param>
        /// <returns>True if allowed, otherwise false</returns>
        public virtual bool IsAccountAllowedToDeletePost(Account account, ForumPost post)
        {
            if (post == null)
                return false;

            return IsAccountAuthorizedForOperation(account, post.AccountId, "DeletePosts");
        }

        /// <summary>
        /// Check whether account is allowed to set topic priority
        /// </summary>
        /// <param name="account">Account</param>
        /// <returns>True if allowed, otherwise false</returns>
        public virtual bool IsAccountAllowedToSetTopicPriority(Account account)
        {
            if (account == null)
            {
                return false;
            }

            if (account.IsGuest())
            {
                return false;
            }

            if (IsForumModerator(account))
            {
                return true;
            }            

            return false;
        }

        /// <summary>
        /// Check whether account is allowed to watch topics
        /// </summary>
        /// <param name="account">Account</param>
        /// <returns>True if allowed, otherwise false</returns>
        public virtual bool IsAccountAllowedToSubscribe(Account account)
        {
            if (account == null)
            {
                return false;
            }

            if (account.IsGuest())
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Calculates topic page index by post identifier
        /// </summary>
        /// <param name="forumTopicId">Forum topic identifier</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="postId">Post identifier</param>
        /// <returns>Page index</returns>
        public virtual int CalculateTopicPageIndex(int forumTopicId, int pageSize, int postId)
        {
            int pageIndex = 0;
            var forumPosts = GetAllPosts(forumTopicId, 0,
                string.Empty, true, 0, int.MaxValue);

            for (int i = 0; i < forumPosts.TotalCount; i++)
            {
                if (forumPosts[i].Id == postId)
                {
                    if (pageSize > 0)
                    {
                        pageIndex = i / pageSize;
                    }
                }
            }

            return pageIndex;
        }
        #endregion
    }
}
