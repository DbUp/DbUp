-- Settings and Statistics

create table SETTINGS(
    ID INTEGER GENERATED ALWAYS AS IDENTITY (start with 1 increment by 1 nocycle),
	NAME VARCHAR2(50) NOT NULL,
    DESCRIPTION VARCHAR2(1000) NOT NULL,
    DISPLAY_NAME VARCHAR2(200) NOT NULL,
    VALUE VARCHAR2(1000) NOT NULL,
    CONSTRAINT PK_SETTING_ID PRIMARY KEY (ID) ENABLE VALIDATE
)
/

INSERT INTO SETTINGS(NAME, DISPLAY_NAME, VALUE, DESCRIPTION) VALUES ('ui-title', 'Title', 'My FunnelWeb Site', 'Text: The title shown at the top in the browser.');
INSERT INTO SETTINGS(NAME, DISPLAY_NAME, VALUE, DESCRIPTION) VALUES ('ui-introduction', 'Introduction', 'Welcome to your FunnelWeb blog. You can <a href="/login">login</a> and edit this message in the administration section. The default username and password is <code>test/test</code>.', 'Markdown: The introductory text that is shown on the home page.');
INSERT INTO SETTINGS(NAME, DISPLAY_NAME, VALUE, DESCRIPTION) VALUES ('ui-links', 'Main Links', '<li><a href="/projects">Projects</a></li>', 'HTML: A list of links shown at the top of each page.');

INSERT INTO SETTINGS(NAME, DISPLAY_NAME, VALUE, DESCRIPTION) VALUES ('search-author', 'Author', 'Daffy Duck', 'Text: Your name.');
INSERT INTO SETTINGS(NAME, DISPLAY_NAME, VALUE, DESCRIPTION) VALUES ('search-keywords', 'Keywords', '.net, c#, test', 'Comma-separated text: Keywords shown to search engines.');
INSERT INTO SETTINGS(NAME, DISPLAY_NAME, VALUE, DESCRIPTION) VALUES ('search-description', 'Description', 'My website.', 'Text: The description shown to search engines in the meta description tag.');
