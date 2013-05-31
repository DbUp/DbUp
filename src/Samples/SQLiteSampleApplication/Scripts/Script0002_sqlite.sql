---------------------------------------
-- Script 0002
-- Initializes Settings
---------------------------------------

INSERT INTO [Setting] 
	([Id], 
	[Name], 
	[DisplayName], 
	[Value], 
	[Description]) 
VALUES 
	(Null, 
	'ui-title', 
	'Title', 
	'My FunnelWeb Site', 
	'Text: The title shown at the top in the browser.');
;      
                       
INSERT INTO [Setting] 
	([Id], 
	[Name], 
	[DisplayName], 
	[Value], 
	[Description]) 
VALUES 
	(Null, 
	'ui-introduction', 
	'Introduction', 
	'Welcome to your FunnelWeb blog. You can <a href="/admin/login">login</a> and edit this message in the administration section. The default username and password is <code>test/test</code>.', 'Markdown: The introductory text that is shown on the home page.');
;      
                       
INSERT INTO [Setting] 
	([Id], 
	[Name], 
	[DisplayName], 
	[Value], 
	[Description]) 
VALUES 
	(Null, 
	'ui-links', 
	'Main Links', 
	'<li><a href="/projects">Projects</a></li>', 'HTML: A list of links shown at the top of each page.');
;                             
                               
INSERT INTO [Setting] 
	([Id], 
	[Name], 
	[DisplayName], 
	[Value], 
	[Description]) 
VALUES 
	(Null, 
	'search-author', 
	'Author', 
	'Daffy Duck', 
	'Text: Your name.');
;      
                       
INSERT INTO [Setting] 
	([Id], 
	[Name], 
	[DisplayName], 
	[Value], 
	[Description]) 
VALUES 
	(Null, 
	'search-keywords', 
	'Keywords', 
	'.net, c#, test', 
	'Comma-separated text: Keywords shown to search engines.');
;
                             
INSERT INTO [Setting] 
	([Id], 
	[Name], 
	[DisplayName], 
	[Value], 
	[Description]) 
VALUES 
	(Null, 
	'search-description', 
	'Description', 
	'My website.', 
	'Text: The description shown to search engines in the meta description tag.');
;

INSERT INTO [Setting]
	([Id], 
	[Name], 
	[DisplayName], 
	[Value], 
	[Description]) 
VALUES 
	(Null, 
	'spam-blacklist', 
	'Spam Blacklist', 
	'casino', 
	'Comments with these words (case-insensitive) will automatically be marked as spam, in addition to Akismet.');
;

INSERT INTO [Setting]
	([Id], 
	[Name], 
	[DisplayName], 
	[Value], 
	[Description]) 
VALUES 
	(Null, 
	'default-page', 
	'Default Page', 
	'blog', 
	'Page name: When users visit the root (/) of your site, it will be equivalent to visiting the page you specify here.');
;

INSERT INTO [Setting]
	([Id], 
	[Name], 
	[DisplayName], 
	[Value], 
	[Description]) 
VALUES 
	(Null, 
	'ui-footer', 
	'Footer', 
	'<p>Powered by <a href="http://www.funnelweblog.com">FunnelWeb</a>, the blog engine of real developers.</p>', 'HTML: This will appear at the bottom of the page - use it to add copyright information, links to any web hosts, people or technologies that helped you to build the site, and so on.');
;

INSERT INTO [Setting]
	([Id], 
	[Name], 
	[DisplayName], 
	[Value], 
	[Description]) 
VALUES 
	(Null, 
	'ui-theme', 
	'Theme', 
	'Clean', 
	'Theme being used by the blog at the moment');
;

INSERT INTO [Setting](
	[Name],
	[DisplayName],
	[Value],
	[Description]
)
VALUES (
	'enable-disqus-comments',
	'Enable Disque Comments',
	'False',
	'Enable the Disqus commenting system. Note - this will still require the theme to also use Disqus.'
);

INSERT INTO [Setting](
	[Id],
	[Name],
	[DisplayName],
	[Value],
	[Description]
)
VALUES (
    Null,
	'disqus-shortname',
	'Shortname for Disqus',
	'',
	'The shortname of your Disqus comments, configured on the Disqus website.'
);
