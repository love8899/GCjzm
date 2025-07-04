﻿using System.Collections.Generic;
using Wfm.Core;
using Wfm.Core.Domain.Accounts;
using Wfm.Core.Domain.Forums;

namespace Wfm.Services.Forums
{
    /// <summary>
    /// Forum service interface
    /// </summary>
    public partial interface IForumService
    {
        /// <summary>
        /// Deletes a forum group
        /// </summary>
        /// <param name="forumGroup">Forum group</param>
        void DeleteForumGroup(ForumGroup forumGroup);

        /// <summary>
        /// Gets a forum group
        /// </summary>
        /// <param name="forumGroupId">The forum group identifier</param>
        /// <returns>Forum group</returns>
        ForumGroup GetForumGroupById(int forumGroupId);

        /// <summary>
        /// Gets all forum groups
        /// </summary>
        /// <returns>Forum groups</returns>
        IList<ForumGroup> GetAllForumGroups();

        /// <summary>
        /// Inserts a forum group
        /// </summary>
        /// <param name="forumGroup">Forum group</param>
        void InsertForumGroup(ForumGroup forumGroup);

        /// <summary>
        /// Updates the forum group
        /// </summary>
        /// <param name="forumGroup">Forum group</param>
        void UpdateForumGroup(ForumGroup forumGroup);

        /// <summary>
        /// Deletes a forum
        /// </summary>
        /// <param name="forum">Forum</param>
        void DeleteForum(Forum forum);

        /// <summary>
        /// Gets a forum
        /// </summary>
        /// <param name="forumId">The forum identifier</param>
        /// <returns>Forum</returns>
        Forum GetForumById(int forumId);

        /// <summary>
        /// Gets forums by group identifier
        /// </summary>
        /// <param name="forumGroupId">The forum group identifier</param>
        /// <returns>Forums</returns>
        IList<Forum> GetAllForumsByGroupId(int forumGroupId);

        /// <summary>
        /// Inserts a forum
        /// </summary>
        /// <param name="forum">Forum</param>
        void InsertForum(Forum forum);

        /// <summary>
        /// Updates the forum
        /// </summary>
        /// <param name="forum">Forum</param>
        void UpdateForum(Forum forum);

        /// <summary>
        /// Deletes a forum topic
        /// </summary>
        /// <param name="forumTopic">Forum topic</param>
        void DeleteTopic(ForumTopic forumTopic);

        /// <summary>
        /// Gets a forum topic
        /// </summary>
        /// <param name="forumTopicId">The forum topic identifier</param>
        /// <returns>Forum Topic</returns>
        ForumTopic GetTopicById(int forumTopicId);

        /// <summary>
        /// Gets a forum topic
        /// </summary>
        /// <param name="forumTopicId">The forum topic identifier</param>
        /// <param name="increaseViews">The value indicating whether to increase forum topic views</param>
        /// <returns>Forum Topic</returns>
        ForumTopic GetTopicById(int forumTopicId, bool increaseViews);

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
        IPagedList<ForumTopic> GetAllTopics(int forumId,
            int accountId, string keywords, ForumSearchType searchType,
            int limitDays, int pageIndex, int pageSize);

        /// <summary>
        /// Gets active forum topics
        /// </summary>
        /// <param name="forumId">The forum identifier</param>
        /// <param name="topicCount">Count of forum topics to return</param>
        /// <returns>Forum Topics</returns>
        IList<ForumTopic> GetActiveTopics(int forumId, int topicCount);

        /// <summary>
        /// Inserts a forum topic
        /// </summary>
        /// <param name="forumTopic">Forum topic</param>
        /// <param name="sendNotifications">A value indicating whether to send notifications to subscribed accounts</param>
        void InsertTopic(ForumTopic forumTopic, bool sendNotifications);

        /// <summary>
        /// Updates the forum topic
        /// </summary>
        /// <param name="forumTopic">Forum topic</param>
        void UpdateTopic(ForumTopic forumTopic);

        /// <summary>
        /// Moves the forum topic
        /// </summary>
        /// <param name="forumTopicId">The forum topic identifier</param>
        /// <param name="newForumId">New forum identifier</param>
        /// <returns>Moved forum topic</returns>
        ForumTopic MoveTopic(int forumTopicId, int newForumId);

        /// <summary>
        /// Deletes a forum post
        /// </summary>
        /// <param name="forumPost">Forum post</param>
        void DeletePost(ForumPost forumPost);

        /// <summary>
        /// Gets a forum post
        /// </summary>
        /// <param name="forumPostId">The forum post identifier</param>
        /// <returns>Forum Post</returns>
        ForumPost GetPostById(int forumPostId);

        /// <summary>
        /// Gets all forum posts
        /// </summary>
        /// <param name="forumTopicId">The forum topic identifier</param>
        /// <param name="accountId">The account identifier</param>
        /// <param name="keywords">Keywords</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Posts</returns>
        IPagedList<ForumPost> GetAllPosts(int forumTopicId,
            int accountId, string keywords, int pageIndex, int pageSize);

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
        IPagedList<ForumPost> GetAllPosts(int forumTopicId, int accountId,
            string keywords, bool ascSort, int pageIndex, int pageSize);

        /// <summary>
        /// Inserts a forum post
        /// </summary>
        /// <param name="forumPost">The forum post</param>
        /// <param name="sendNotifications">A value indicating whether to send notifications to subscribed accounts</param>
        void InsertPost(ForumPost forumPost, bool sendNotifications);

        /// <summary>
        /// Updates the forum post
        /// </summary>
        /// <param name="forumPost">Forum post</param>
        void UpdatePost(ForumPost forumPost);

        /// <summary>
        /// Deletes a private message
        /// </summary>
        /// <param name="privateMessage">Private message</param>
        void DeletePrivateMessage(PrivateMessage privateMessage);

        /// <summary>
        /// Gets a private message
        /// </summary>
        /// <param name="privateMessageId">The private message identifier</param>
        /// <returns>Private message</returns>
        PrivateMessage GetPrivateMessageById(int privateMessageId);

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
        IPagedList<PrivateMessage> GetAllPrivateMessages(int franchiseId, int fromAccountId,
            int toAccountId, bool? isRead, bool? isDeletedByAuthor, bool? isDeletedByRecipient,
            string keywords, int pageIndex, int pageSize);

        /// <summary>
        /// Inserts a private message
        /// </summary>
        /// <param name="privateMessage">Private message</param>
        void InsertPrivateMessage(PrivateMessage privateMessage);

        /// <summary>
        /// Updates the private message
        /// </summary>
        /// <param name="privateMessage">Private message</param>
        void UpdatePrivateMessage(PrivateMessage privateMessage);

        /// <summary>
        /// Deletes a forum subscription
        /// </summary>
        /// <param name="forumSubscription">Forum subscription</param>
        void DeleteSubscription(ForumSubscription forumSubscription);

        /// <summary>
        /// Gets a forum subscription
        /// </summary>
        /// <param name="forumSubscriptionId">The forum subscription identifier</param>
        /// <returns>Forum subscription</returns>
        ForumSubscription GetSubscriptionById(int forumSubscriptionId);

        /// <summary>
        /// Gets forum subscriptions
        /// </summary>
        /// <param name="accountId">The account identifier</param>
        /// <param name="forumId">The forum identifier</param>
        /// <param name="topicId">The topic identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Forum subscriptions</returns>
        IPagedList<ForumSubscription> GetAllSubscriptions(int accountId, int forumId,
            int topicId, int pageIndex, int pageSize);

        /// <summary>
        /// Inserts a forum subscription
        /// </summary>
        /// <param name="forumSubscription">Forum subscription</param>
        void InsertSubscription(ForumSubscription forumSubscription);

        /// <summary>
        /// Updates the forum subscription
        /// </summary>
        /// <param name="forumSubscription">Forum subscription</param>
        void UpdateSubscription(ForumSubscription forumSubscription);

        /// <summary>
        /// Check whether account is allowed to create new topics
        /// </summary>
        /// <param name="account">Account</param>
        /// <param name="forum">Forum</param>
        /// <returns>True if allowed, otherwise false</returns>
        bool IsAccountAllowedToCreateTopic(Account account, Forum forum);

        /// <summary>
        /// Check whether account is allowed to edit topic
        /// </summary>
        /// <param name="account">Account</param>
        /// <param name="topic">Topic</param>
        /// <returns>True if allowed, otherwise false</returns>
        bool IsAccountAllowedToEditTopic(Account account, ForumTopic topic);

        /// <summary>
        /// Check whether account is allowed to move topic
        /// </summary>
        /// <param name="account">Account</param>
        /// <param name="topic">Topic</param>
        /// <returns>True if allowed, otherwise false</returns>
        bool IsAccountAllowedToMoveTopic(Account account, ForumTopic topic);

        /// <summary>
        /// Check whether account is allowed to delete topic
        /// </summary>
        /// <param name="account">Account</param>
        /// <param name="topic">Topic</param>
        /// <returns>True if allowed, otherwise false</returns>
        bool IsAccountAllowedToDeleteTopic(Account account, ForumTopic topic);

        /// <summary>
        /// Check whether account is allowed to create new post
        /// </summary>
        /// <param name="account">Account</param>
        /// <param name="topic">Topic</param>
        /// <returns>True if allowed, otherwise false</returns>
        bool IsAccountAllowedToCreatePost(Account account, ForumTopic topic);

        /// <summary>
        /// Check whether account is allowed to edit post
        /// </summary>
        /// <param name="account">Account</param>
        /// <param name="post">Topic</param>
        /// <returns>True if allowed, otherwise false</returns>
        bool IsAccountAllowedToEditPost(Account account, ForumPost post);

        /// <summary>
        /// Check whether account is allowed to delete post
        /// </summary>
        /// <param name="account">Account</param>
        /// <param name="post">Topic</param>
        /// <returns>True if allowed, otherwise false</returns>
        bool IsAccountAllowedToDeletePost(Account account, ForumPost post);

        /// <summary>
        /// Check whether account is allowed to set topic priority
        /// </summary>
        /// <param name="account">Account</param>
        /// <returns>True if allowed, otherwise false</returns>
        bool IsAccountAllowedToSetTopicPriority(Account account);

        /// <summary>
        /// Check whether account is allowed to watch topics
        /// </summary>
        /// <param name="account">Account</param>
        /// <returns>True if allowed, otherwise false</returns>
        bool IsAccountAllowedToSubscribe(Account account);

        /// <summary>
        /// Calculates topic page index by post identifier
        /// </summary>
        /// <param name="forumTopicId">Topic identifier</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="postId">Post identifier</param>
        /// <returns>Page index</returns>
        int CalculateTopicPageIndex(int forumTopicId, int pageSize, int postId);
    }
}
