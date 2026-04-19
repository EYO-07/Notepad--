// {TextMarker|cyan:>>>,<<<,TODO|red:ISSUE|yellow:INCOMPLETE,DEPRECATED}
// -- BEGIN 
// CODEX_DarkQtScintilla.cpp
#include "CODEX_DarkQtScintilla.h"
#include <Qsci/qscicommandset.h>
#include <Qsci/qscicommand.h>
#include <QShortcut>      
#include <QKeySequence>   
#include <QSet>
using namespace CodexTransmutation;
using namespace CodexIncantation;

// variables
QColor bgColor(0, 0, 0, 120); 
QColor fgColor("white"); 
QColor caretColor("#528bff");
QColor selectionColor("#3e4451");
QColor stringColor(255,100,100);
QColor stringPaperColor(50,0,0);
QColor commentColor(0, 204, 153);
QColor commentPaperColor(0, 50, 0);
QColor numberColor(0,255,255);
QColor numberPaperColor(0,0,100);
QColor braceMatchColor("cyan");
QColor braceMatchBackgroundColor(0,0,0);
QColor operatorColor(230,230,0);
QColor preprocessorColor(153, 153, 102);
QColor keyword1("silver");
QColor keyword2("lightblue");
QColor classColor("lightblue");
QColor functionColor("lightblue");
QFont FONT("Monospace",8,QFont::Bold);
QFont MARGIN_FONT("Monospace",8);
QSet<QString> FILE_EXT_LEXER_CPP = {"h", "hpp", "hxx", "hh","c", "cpp", "cxx", "cc"};
QSet<QString> FILE_EXT_LEXER_CS = {"cs", "csx"};
QSet<QString> FILE_EXT_LEXER_PYTHON = {"py", "pyw"};
QSet<QString> FILE_EXT_LEXER_JAVA = {"java"};
QSet<QString> FILE_EXT_LEXER_JS = {"js", "mjs", "cjs"};
QSet<QString> FILE_EXT_LEXER_TS = {"ts", "tsx"};
QSet<QString> FILE_EXT_LEXER_HTML = {"html", "htm", "xhtml"};
QSet<QString> FILE_EXT_LEXER_XML = {"xml", "xsd", "xslt", "svg"};
QSet<QString> FILE_EXT_LEXER_CSS = {"css", "scss", "sass", "less"};
QSet<QString> FILE_EXT_LEXER_JSON = {"json"};
QSet<QString> FILE_EXT_LEXER_SHELL = {"sh", "bash", "zsh", "ksh"};
QSet<QString> FILE_EXT_LEXER_BATCH = {"bat", "cmd"};
QSet<QString> FILE_EXT_LEXER_SQL = {"sql"};
QSet<QString> FILE_EXT_LEXER_PHP = {"php", "phtml", "php3", "php4", "php5", "php7", "php8"};
QSet<QString> FILE_EXT_LEXER_RUBY = {"rb", "erb", "rake"};
QSet<QString> FILE_EXT_LEXER_PERL = {"pl", "pm", "t"};
QSet<QString> FILE_EXT_LEXER_LUA = {"lua"};
QSet<QString> FILE_EXT_LEXER_MARKDOWN = {"md", "markdown"};
QSet<QString> FILE_EXT_LEXER_YAML = {"yaml", "yml"};
QSet<QString> FILE_EXT_LEXER_INI = {"ini", "cfg", "conf"};
QHash<QsciScintilla*, QHash<QString, QVariant>> SCINTILLA_DATA;
QRegularExpression directiveRegex(R"(\{TextMarker\|([^}]+)\})");

// ===================================================================================

// Incantation 

// -- local functions 
void lexerPostSettings(QsciScintilla* editor) {
    editor->setAutoIndent(true);
    editor->setIndentationsUseTabs(false);
    editor->setTabWidth(4);
    editor->setIndentationGuides(true);
    editor->setCaretLineVisible(true);        
    editor->setWrapMode(QsciScintilla::WrapWord);
    editor->setBraceMatching(QsciScintilla::StrictBraceMatch);
    editor->setCaretLineBackgroundColor(QColor("#2c313c"));
    editor->setCaretForegroundColor(caretColor);
    editor->setMarginType(1, QsciScintilla::NumberMargin);
    editor->setMarginLineNumbers(1, true);
    editor->setMarginWidth(1, "0000");
    editor->setMarginsFont(MARGIN_FONT);
    editor->setMarginsBackgroundColor(QColor(10,10,10));
    editor->setMarginsForegroundColor(fgColor);
    editor->setMatchedBraceForegroundColor(braceMatchColor);
    editor->setMatchedBraceBackgroundColor(braceMatchBackgroundColor);
    editor->setUnmatchedBraceBackgroundColor(bgColor);
    editor->setUnmatchedBraceForegroundColor(operatorColor);
    editor->setSelectionBackgroundColor(selectionColor);
    editor->setUtf8(true);
    editor->lexer()->setFont(FONT);
    editor->setFont(FONT);
    editor->SendScintilla(QsciScintilla::SCI_SETEXTRAASCENT, 0);
    editor->SendScintilla(QsciScintilla::SCI_SETEXTRADESCENT, 0);
    // 1. Set the selection background color (e.g., a nice blue)
    //QColor selBack("#264f78"); 
    //editor->SendScintilla(QsciScintilla::SCI_SETLISTSELBACK, selBack.rgb());
    // 2. Set the selection foreground color (e.g., white text)
    //QColor selFore("#ffffff");
    //editor->SendScintilla(QsciScintilla::SCI_SETLISTSELFORE, selFore.rgb());
}
void registerEditor(QsciScintilla* editor) {
    if (!editor) return;
    if ( SCINTILLA_DATA.contains(editor) ) return;
    // SCINTILLA_DATA[editor] = QHash<QString, QVariant>(); // not necessary
    QObject::connect(editor, &QObject::destroyed, [editor]() {
        SCINTILLA_DATA.remove(editor);
    });
}
void clearIndicators(QsciScintilla* editor) {
    for (int id = 8; id <= 20; ++id) {
        editor->clearIndicatorRange(0, 0, editor->lines() - 1, editor->lineLength(editor->lines() - 1), id);
    }
}
void setIndicator(QsciScintilla* editor, int indicatorId, QColor color) { // ISSUE full box missing bottom line 
    editor->indicatorDefine(QsciScintilla::StraightBoxIndicator, indicatorId);
    editor->setIndicatorForegroundColor(color, indicatorId);
    editor->SendScintilla(QsciScintilla::SCI_SETINDICATORCURRENT, indicatorId);
    editor->SendScintilla(QsciScintilla::SCI_INDICSETALPHA, indicatorId, 60);
    editor->SendScintilla(QsciScintilla::SCI_INDICSETOUTLINEALPHA, indicatorId, 255);
    editor->SendScintilla(QsciScintilla::SCI_INDICSETUNDER, indicatorId, true);
}
void applyCPPStyle(QsciLexerCPP* lexer) {
    // -- cpp style 
    lexer->setHighlightTripleQuotedStrings(true);
    lexer->setHighlightHashQuotedStrings(true);
    lexer->setHighlightBackQuotedStrings(true);
    lexer->setColor(fgColor, QsciLexerCPP::Default);
    lexer->setPaper(bgColor, QsciLexerCPP::Default);
    lexer->setColor(commentColor, QsciLexerCPP::Comment); 
    lexer->setPaper(commentPaperColor, QsciLexerCPP::Comment);
    lexer->setColor(commentColor, QsciLexerCPP::CommentLine);
    lexer->setPaper(commentPaperColor, QsciLexerCPP::CommentLine);
    lexer->setColor(commentColor, QsciLexerCPP::CommentDoc); 
    lexer->setPaper(commentPaperColor, QsciLexerCPP::CommentDoc);
    lexer->setColor(numberColor, QsciLexerCPP::Number); 
    lexer->setPaper(numberPaperColor, QsciLexerCPP::Number);
    lexer->setColor(keyword1, QsciLexerCPP::Keyword);      
    lexer->setColor(stringColor, QsciLexerCPP::DoubleQuotedString); 
    lexer->setPaper(stringPaperColor, QsciLexerCPP::DoubleQuotedString); 
    lexer->setColor(stringColor, QsciLexerCPP::SingleQuotedString); 
    lexer->setPaper(stringPaperColor, QsciLexerCPP::SingleQuotedString); 
    lexer->setColor(preprocessorColor, QsciLexerCPP::PreProcessor); 
    lexer->setColor(operatorColor, QsciLexerCPP::Operator); 
    lexer->setColor(fgColor, QsciLexerCPP::Identifier); 
    lexer->setColor(stringColor, QsciLexerCPP::UnclosedString); 
    lexer->setPaper(stringPaperColor, QsciLexerCPP::UnclosedString); 
    lexer->setColor(stringColor, QsciLexerCPP::VerbatimString); 
    lexer->setPaper(stringPaperColor, QsciLexerCPP::VerbatimString); 
    lexer->setColor(stringColor, QsciLexerCPP::Regex); 
    lexer->setPaper(stringPaperColor, QsciLexerCPP::Regex); 
    lexer->setColor(commentColor, QsciLexerCPP::CommentLineDoc); 
    lexer->setPaper(commentPaperColor, QsciLexerCPP::CommentLineDoc);
    lexer->setColor(keyword2, QsciLexerCPP::KeywordSet2);   
    lexer->setColor(commentColor, QsciLexerCPP::CommentDocKeyword); 
    lexer->setPaper(commentPaperColor, QsciLexerCPP::CommentDocKeyword);
    lexer->setColor(commentColor, QsciLexerCPP::CommentDocKeywordError); 
    lexer->setPaper(commentPaperColor, QsciLexerCPP::CommentDocKeywordError);
    lexer->setColor(stringColor, QsciLexerCPP::RawString);
    lexer->setPaper(stringPaperColor, QsciLexerCPP::RawString); 
    lexer->setColor(stringColor, QsciLexerCPP::TripleQuotedVerbatimString);
    lexer->setPaper(stringPaperColor, QsciLexerCPP::TripleQuotedVerbatimString);
}
void applyHTMLStyle(QsciLexerHTML* lexer) {
    lexer->setColor(fgColor, QsciLexerHTML::Default);
    lexer->setPaper(bgColor, QsciLexerHTML::Default);
    lexer->setColor(keyword1, QsciLexerHTML::Tag); 
    lexer->setColor(fgColor, QsciLexerHTML::UnknownTag); 
    lexer->setColor(numberColor, QsciLexerHTML::HTMLNumber); 
    lexer->setPaper(numberPaperColor, QsciLexerHTML::HTMLNumber);
    lexer->setColor(stringColor, QsciLexerHTML::HTMLDoubleQuotedString); 
    lexer->setPaper(stringPaperColor, QsciLexerHTML::HTMLDoubleQuotedString); 
    lexer->setColor(operatorColor, QsciLexerHTML::OtherInTag); 
    lexer->setColor(commentColor, QsciLexerHTML::HTMLComment); 
    lexer->setPaper(commentPaperColor, QsciLexerHTML::HTMLComment);
    lexer->setColor(fgColor, QsciLexerHTML::Entity);
    lexer->setColor(fgColor, QsciLexerHTML::XMLTagEnd); 
    lexer->setColor(fgColor, QsciLexerHTML::XMLStart); 
    lexer->setColor(fgColor, QsciLexerHTML::XMLEnd); 
    lexer->setColor(fgColor, QsciLexerHTML::Script); 
    lexer->setColor(fgColor, QsciLexerHTML::ASPAtStart); 
    //lexer->setColor(fgColor, QsciLexerHTML::ASPAtEnd); 
    lexer->setColor(fgColor, QsciLexerHTML::CDATA); 
    lexer->setColor(fgColor, QsciLexerHTML::PHPStart); 
    lexer->setColor(numberColor, QsciLexerHTML::HTMLValue); 
    lexer->setPaper(numberPaperColor, QsciLexerHTML::HTMLValue);
    lexer->setColor(commentColor, QsciLexerHTML::ASPXCComment); 
    lexer->setPaper(commentPaperColor, QsciLexerHTML::ASPXCComment);
    //
    lexer->setColor(fgColor, QsciLexerHTML::SGMLDefault);
    lexer->setPaper(bgColor, QsciLexerHTML::SGMLDefault);
    lexer->setColor(fgColor, QsciLexerHTML::SGMLParameter);
    lexer->setColor(stringColor, QsciLexerHTML::SGMLDoubleQuotedString); 
    lexer->setPaper(stringPaperColor, QsciLexerHTML::SGMLDoubleQuotedString); 
    lexer->setColor(stringColor, QsciLexerHTML::SGMLSingleQuotedString); 
    lexer->setPaper(stringPaperColor, QsciLexerHTML::SGMLSingleQuotedString); 
    lexer->setColor(fgColor, QsciLexerHTML::SGMLError);
    lexer->setPaper(bgColor, QsciLexerHTML::SGMLError);
    lexer->setColor(fgColor, QsciLexerHTML::SGMLSpecial);
    lexer->setColor(fgColor, QsciLexerHTML::SGMLEntity);
    lexer->setColor(commentColor, QsciLexerHTML::SGMLComment); 
    lexer->setPaper(commentPaperColor, QsciLexerHTML::SGMLComment);
    lexer->setColor(commentColor, QsciLexerHTML::SGMLParameterComment); 
    lexer->setPaper(commentPaperColor, QsciLexerHTML::SGMLParameterComment);
    lexer->setColor(fgColor, QsciLexerHTML::SGMLBlockDefault);
    //
    lexer->setColor(fgColor, QsciLexerHTML::JavaScriptStart);
    lexer->setColor(fgColor, QsciLexerHTML::JavaScriptDefault);
    lexer->setColor(commentColor, QsciLexerHTML::JavaScriptComment); 
    lexer->setPaper(commentPaperColor, QsciLexerHTML::JavaScriptComment);
    lexer->setColor(commentColor, QsciLexerHTML::JavaScriptCommentLine); 
    lexer->setPaper(commentPaperColor, QsciLexerHTML::JavaScriptCommentLine);
    lexer->setColor(commentColor, QsciLexerHTML::JavaScriptCommentDoc); 
    lexer->setPaper(commentPaperColor, QsciLexerHTML::JavaScriptCommentDoc);
    lexer->setColor(numberColor, QsciLexerHTML::JavaScriptNumber); 
    lexer->setPaper(numberPaperColor, QsciLexerHTML::JavaScriptNumber);
    lexer->setColor(keyword1, QsciLexerHTML::JavaScriptWord); 
    lexer->setColor(keyword2, QsciLexerHTML::JavaScriptKeyword); 
    lexer->setColor(stringColor, QsciLexerHTML::JavaScriptDoubleQuotedString); 
    lexer->setPaper(stringPaperColor, QsciLexerHTML::JavaScriptDoubleQuotedString); 
    lexer->setColor(stringColor, QsciLexerHTML::JavaScriptSingleQuotedString); 
    lexer->setPaper(stringPaperColor, QsciLexerHTML::JavaScriptSingleQuotedString); 
    lexer->setColor(numberColor, QsciLexerHTML::JavaScriptSymbol); 
    lexer->setPaper(numberPaperColor, QsciLexerHTML::JavaScriptSymbol);
    lexer->setColor(stringColor, QsciLexerHTML::JavaScriptUnclosedString); 
    lexer->setPaper(stringPaperColor, QsciLexerHTML::JavaScriptUnclosedString); 
    lexer->setColor(stringColor, QsciLexerHTML::JavaScriptRegex); 
    lexer->setPaper(stringPaperColor, QsciLexerHTML::JavaScriptRegex); 
    //
    lexer->setColor(fgColor, QsciLexerHTML::ASPJavaScriptStart);
    lexer->setColor(fgColor, QsciLexerHTML::ASPJavaScriptDefault);
    lexer->setColor(commentColor, QsciLexerHTML::ASPJavaScriptComment); 
    lexer->setPaper(commentPaperColor, QsciLexerHTML::ASPJavaScriptComment);
    lexer->setColor(commentColor, QsciLexerHTML::ASPJavaScriptCommentLine); 
    lexer->setPaper(commentPaperColor, QsciLexerHTML::ASPJavaScriptCommentLine);
    lexer->setColor(commentColor, QsciLexerHTML::ASPJavaScriptCommentDoc); 
    lexer->setPaper(commentPaperColor, QsciLexerHTML::ASPJavaScriptCommentDoc);
    lexer->setColor(numberColor, QsciLexerHTML::ASPJavaScriptNumber); 
    lexer->setPaper(numberPaperColor, QsciLexerHTML::ASPJavaScriptNumber);
    lexer->setColor(keyword1, QsciLexerHTML::ASPJavaScriptWord); 
    lexer->setColor(keyword2, QsciLexerHTML::ASPJavaScriptKeyword); 
    lexer->setColor(stringColor, QsciLexerHTML::ASPJavaScriptDoubleQuotedString); 
    lexer->setPaper(stringPaperColor, QsciLexerHTML::ASPJavaScriptDoubleQuotedString); 
    lexer->setColor(stringColor, QsciLexerHTML::ASPJavaScriptSingleQuotedString); 
    lexer->setPaper(stringPaperColor, QsciLexerHTML::ASPJavaScriptSingleQuotedString); 
    lexer->setColor(numberColor, QsciLexerHTML::ASPJavaScriptSymbol); 
    lexer->setPaper(numberPaperColor, QsciLexerHTML::ASPJavaScriptSymbol);
    lexer->setColor(stringColor, QsciLexerHTML::ASPJavaScriptUnclosedString); 
    lexer->setPaper(stringPaperColor, QsciLexerHTML::ASPJavaScriptUnclosedString); 
    lexer->setColor(stringColor, QsciLexerHTML::ASPJavaScriptRegex); 
    lexer->setPaper(stringPaperColor, QsciLexerHTML::ASPJavaScriptRegex); 
    // 
    lexer->setColor(fgColor, QsciLexerHTML::VBScriptStart);
    lexer->setColor(fgColor, QsciLexerHTML::VBScriptDefault);
    lexer->setColor(commentColor, QsciLexerHTML::VBScriptComment); 
    lexer->setPaper(commentPaperColor, QsciLexerHTML::VBScriptComment);
    lexer->setColor(numberColor, QsciLexerHTML::VBScriptNumber); 
    lexer->setPaper(numberPaperColor, QsciLexerHTML::VBScriptNumber);
    lexer->setColor(keyword2, QsciLexerHTML::VBScriptKeyword); 
    lexer->setColor(stringColor, QsciLexerHTML::VBScriptString); 
    lexer->setPaper(stringPaperColor, QsciLexerHTML::VBScriptString); 
    lexer->setColor(fgColor, QsciLexerHTML::VBScriptIdentifier);
    lexer->setColor(stringColor, QsciLexerHTML::VBScriptUnclosedString); 
    lexer->setPaper(stringPaperColor, QsciLexerHTML::VBScriptUnclosedString); 
    //
    lexer->setColor(fgColor, QsciLexerHTML::ASPVBScriptStart);
    lexer->setColor(fgColor, QsciLexerHTML::ASPVBScriptDefault);
    lexer->setColor(commentColor, QsciLexerHTML::ASPVBScriptComment); 
    lexer->setPaper(commentPaperColor, QsciLexerHTML::ASPVBScriptComment);
    lexer->setColor(numberColor, QsciLexerHTML::ASPVBScriptNumber); 
    lexer->setPaper(numberPaperColor, QsciLexerHTML::ASPVBScriptNumber);
    lexer->setColor(keyword2, QsciLexerHTML::ASPVBScriptKeyword); 
    lexer->setColor(stringColor, QsciLexerHTML::ASPVBScriptString); 
    lexer->setPaper(stringPaperColor, QsciLexerHTML::ASPVBScriptString); 
    lexer->setColor(fgColor, QsciLexerHTML::ASPVBScriptIdentifier);
    lexer->setColor(stringColor, QsciLexerHTML::ASPVBScriptUnclosedString); 
    lexer->setPaper(stringPaperColor, QsciLexerHTML::ASPVBScriptUnclosedString); 
    //
    lexer->setColor(fgColor, QsciLexerHTML::PythonStart);
    lexer->setColor(fgColor, QsciLexerHTML::PythonDefault);
    lexer->setColor(commentColor, QsciLexerHTML::PythonComment); 
    lexer->setPaper(commentPaperColor, QsciLexerHTML::PythonComment);
    lexer->setColor(numberColor, QsciLexerHTML::PythonNumber); 
    lexer->setPaper(numberPaperColor, QsciLexerHTML::PythonNumber);
    lexer->setColor(stringColor, QsciLexerHTML::PythonDoubleQuotedString); 
    lexer->setPaper(stringPaperColor, QsciLexerHTML::PythonDoubleQuotedString); 
    lexer->setColor(stringColor, QsciLexerHTML::PythonSingleQuotedString); 
    lexer->setPaper(stringPaperColor, QsciLexerHTML::PythonSingleQuotedString); 
    lexer->setColor(stringColor, QsciLexerHTML::PythonTripleSingleQuotedString); 
    lexer->setPaper(stringPaperColor, QsciLexerHTML::PythonTripleSingleQuotedString); 
    lexer->setColor(stringColor, QsciLexerHTML::PythonTripleDoubleQuotedString); 
    lexer->setPaper(stringPaperColor, QsciLexerHTML::PythonTripleDoubleQuotedString); 
    lexer->setColor(classColor, QsciLexerHTML::PythonClassName); 
    lexer->setColor(functionColor, QsciLexerHTML::PythonFunctionMethodName); 
    lexer->setColor(operatorColor, QsciLexerHTML::PythonOperator); 
    lexer->setColor(fgColor, QsciLexerHTML::PythonIdentifier);
    //
    lexer->setColor(fgColor, QsciLexerHTML::ASPPythonStart);
    lexer->setColor(fgColor, QsciLexerHTML::ASPPythonDefault);
    lexer->setColor(commentColor, QsciLexerHTML::ASPPythonComment); 
    lexer->setPaper(commentPaperColor, QsciLexerHTML::ASPPythonComment);
    lexer->setColor(numberColor, QsciLexerHTML::ASPPythonNumber); 
    lexer->setPaper(numberPaperColor, QsciLexerHTML::ASPPythonNumber);
    lexer->setColor(stringColor, QsciLexerHTML::ASPPythonDoubleQuotedString); 
    lexer->setPaper(stringPaperColor, QsciLexerHTML::ASPPythonDoubleQuotedString); 
    lexer->setColor(stringColor, QsciLexerHTML::ASPPythonSingleQuotedString); 
    lexer->setPaper(stringPaperColor, QsciLexerHTML::ASPPythonSingleQuotedString); 
    lexer->setColor(stringColor, QsciLexerHTML::ASPPythonTripleSingleQuotedString); 
    lexer->setPaper(stringPaperColor, QsciLexerHTML::ASPPythonTripleSingleQuotedString); 
    lexer->setColor(stringColor, QsciLexerHTML::ASPPythonTripleDoubleQuotedString); 
    lexer->setPaper(stringPaperColor, QsciLexerHTML::ASPPythonTripleDoubleQuotedString); 
    lexer->setColor(classColor, QsciLexerHTML::ASPPythonClassName); 
    lexer->setColor(functionColor, QsciLexerHTML::ASPPythonFunctionMethodName); 
    lexer->setColor(operatorColor, QsciLexerHTML::ASPPythonOperator); 
    lexer->setColor(fgColor, QsciLexerHTML::ASPPythonIdentifier);
    //
    lexer->setColor(fgColor, QsciLexerHTML::PHPDefault);
    lexer->setColor(stringColor, QsciLexerHTML::PHPDoubleQuotedString); 
    lexer->setPaper(stringPaperColor, QsciLexerHTML::PHPDoubleQuotedString); 
    lexer->setColor(stringColor, QsciLexerHTML::PHPSingleQuotedString); 
    lexer->setPaper(stringPaperColor, QsciLexerHTML::PHPSingleQuotedString); 
    lexer->setColor(keyword2, QsciLexerHTML::PHPKeyword); 
    lexer->setColor(numberColor, QsciLexerHTML::PHPNumber); 
    lexer->setPaper(numberPaperColor, QsciLexerHTML::PHPNumber);
    lexer->setColor(fgColor, QsciLexerHTML::PHPVariable);
    lexer->setColor(commentColor, QsciLexerHTML::PHPComment); 
    lexer->setPaper(commentPaperColor, QsciLexerHTML::PHPComment);
    lexer->setColor(commentColor, QsciLexerHTML::PHPCommentLine); 
    lexer->setPaper(commentPaperColor, QsciLexerHTML::PHPCommentLine);
    lexer->setColor(fgColor, QsciLexerHTML::PHPDoubleQuotedVariable);
    lexer->setColor(operatorColor, QsciLexerHTML::PHPOperator); 
}

// -- implementations
QsciScintilla* CodexIncantation::newDarkScintilla(QWidget *parent, QString fileName) {
    QsciScintilla *editor = new QsciScintilla(parent);
    editor->viewport()->setAutoFillBackground(false);
    editor->setAutoFillBackground(false);
    editor->setAttribute(Qt::WA_TranslucentBackground);
    editor->setAttribute(Qt::WA_OpaquePaintEvent, false);
    editor->setUtf8(true);
    editor->setFont(FONT);
    //editor->setStyleSheet("border: none; background-color: rgba(0,0,0,120);");
    // CRITICAL: Some lexers need this property set via Scintilla's internal API
    editor->SendScintilla(QsciScintilla::SCI_SETPROPERTY, "fold", "1");
    editor->SendScintilla(QsciScintilla::SCI_SETPROPERTY, "fold.compact", "0");
    editor->SendScintilla(QsciScintilla::SCI_SETBUFFEREDDRAW, true);
    //
    CodexIncantation::setLexer(editor, fileName);
    CodexIncantation::setLexerFolding(editor, fileName);
    lexerPostSettings(editor);
    CodexIncantation::setAutocompletion(editor);
    return editor;
}
void CodexIncantation::onTextChange(QsciScintilla* editor, std::function<void(QsciScintilla*)> logic) {
    if (!editor) return;
    QObject::connect(editor, &QsciScintilla::textChanged, editor, [editor, logic]() {
        if (logic) {
            logic(editor);
        }
    });
}
void CodexIncantation::toggleCurrentFold(QsciScintilla *editor) {
    int line = editor->SendScintilla(QsciScintilla::SCI_LINEFROMPOSITION, editor->SendScintilla(QsciScintilla::SCI_GETCURRENTPOS));
    editor->SendScintilla(QsciScintilla::SCI_TOGGLEFOLD, line);
}
bool CodexIncantation::setLexer(QsciScintilla* editor, QString fileName) { // INCOMPLETE
    if (!editor) return false;
    // Logic [ setLexer ] -> setLexer() || $ext | %% choose lexer 
    // setLexer() || $ext
    QString ext = getExtension(fileName);
    // setLexer() || $ext | %% choose lexer 
    // } else if ( .contains(ext) ) {
    if ( FILE_EXT_LEXER_CPP.contains(ext) ) {
        QsciLexerCPP* lexer = new QsciLexerCPP(editor);
        lexer->setDefaultPaper(bgColor);
        lexer->setDefaultColor(fgColor);
        lexer->setFoldComments(true);
        lexer->setFoldPreprocessor(true);
        lexer->setFoldAtElse(true);
        lexer->setFoldCompact(false);
        applyCPPStyle(lexer);
        editor->setLexer(lexer);
        return true;
    } else if ( FILE_EXT_LEXER_PYTHON.contains(ext) ) {
        QsciLexerPython* lexer = new QsciLexerPython(editor);
        lexer->setDefaultPaper(bgColor);
        lexer->setDefaultColor(fgColor);
        lexer->setFoldComments(true);
        //lexer->setFoldPreprocessor(true);
        //lexer->setFoldAtElse(true);
        lexer->setFoldCompact(false);
        // -- Python style 
        lexer->setColor(fgColor, QsciLexerPython::Default);
        lexer->setPaper(bgColor, QsciLexerPython::Default);
        lexer->setColor(commentColor, QsciLexerPython::Comment); 
        lexer->setPaper(commentPaperColor, QsciLexerPython::Comment);
        lexer->setColor(numberColor, QsciLexerPython::Number); 
        lexer->setPaper(numberPaperColor, QsciLexerPython::Number);
        lexer->setColor(keyword1, QsciLexerPython::Keyword); 
        lexer->setColor(stringColor, QsciLexerPython::DoubleQuotedString); 
        lexer->setPaper(stringPaperColor, QsciLexerPython::DoubleQuotedString); 
        lexer->setColor(stringColor, QsciLexerPython::SingleQuotedString); 
        lexer->setPaper(stringPaperColor, QsciLexerPython::SingleQuotedString); 
        lexer->setColor(operatorColor, QsciLexerPython::Operator); 
        lexer->setColor(fgColor, QsciLexerPython::Identifier); 
        lexer->setColor(stringColor, QsciLexerPython::UnclosedString); 
        lexer->setPaper(stringPaperColor, QsciLexerPython::UnclosedString); 
        lexer->setColor(preprocessorColor, QsciLexerPython::Decorator);
        lexer->setColor(stringColor, QsciLexerPython::DoubleQuotedFString);
        lexer->setPaper(stringPaperColor, QsciLexerPython::DoubleQuotedFString);
        lexer->setColor(stringColor, QsciLexerPython::SingleQuotedFString);
        lexer->setPaper(stringPaperColor, QsciLexerPython::SingleQuotedFString);
        lexer->setColor(classColor, QsciLexerPython::ClassName);
        lexer->setColor(functionColor, QsciLexerPython::FunctionMethodName);
        //
        editor->setLexer(lexer);
        return true;
    } else if ( FILE_EXT_LEXER_JAVA.contains(ext) ) {
        QsciLexerJava* lexer = new QsciLexerJava(editor);
        lexer->setDefaultPaper(bgColor);
        lexer->setDefaultColor(fgColor);
        lexer->setFoldComments(true);
        lexer->setFoldPreprocessor(true);
        lexer->setFoldAtElse(true);
        lexer->setFoldCompact(false);
        applyCPPStyle(lexer);
        editor->setLexer(lexer);
        return true;
    } else if ( FILE_EXT_LEXER_JS.contains(ext) ) {
        QsciLexerJavaScript* lexer = new QsciLexerJavaScript(editor);
        lexer->setDefaultPaper(bgColor);
        lexer->setDefaultColor(fgColor);
        lexer->setFoldComments(true);
        lexer->setFoldPreprocessor(true);
        lexer->setFoldAtElse(true);
        lexer->setFoldCompact(false);
        applyCPPStyle(lexer);
        editor->setLexer(lexer);
        return true;
    } else if ( FILE_EXT_LEXER_TS.contains(ext) ) {
        QsciLexerJavaScript* lexer = new QsciLexerJavaScript(editor);
        lexer->setDefaultPaper(bgColor);
        lexer->setDefaultColor(fgColor);
        lexer->setFoldComments(true);
        lexer->setFoldPreprocessor(true);
        lexer->setFoldAtElse(true);
        lexer->setFoldCompact(false);
        applyCPPStyle(lexer);
        editor->setLexer(lexer);
        return true;
    } else if ( FILE_EXT_LEXER_HTML.contains(ext) ) {
        QsciLexerHTML* lexer = new QsciLexerHTML(editor);
        lexer->setDefaultPaper(bgColor);
        lexer->setDefaultColor(fgColor);
        //lexer->setFoldComments(true);
        lexer->setFoldPreprocessor(true);
        //lexer->setFoldAtElse(true);
        lexer->setFoldCompact(false);
        applyHTMLStyle(lexer);
        editor->setLexer(lexer);
        return true;
    } else if ( FILE_EXT_LEXER_XML.contains(ext) ) {
        QsciLexerXML* lexer = new QsciLexerXML(editor);
        lexer->setDefaultPaper(bgColor);
        lexer->setDefaultColor(fgColor);
        //lexer->setFoldComments(true);
        lexer->setFoldPreprocessor(true);
        //lexer->setFoldAtElse(true);
        lexer->setFoldCompact(false);
        applyHTMLStyle(lexer);
        editor->setLexer(lexer);
        return true;
    } else if ( FILE_EXT_LEXER_CSS.contains(ext) ) {
        QsciLexerCSS* lexer = new QsciLexerCSS(editor);
        lexer->setDefaultPaper(bgColor);
        lexer->setDefaultColor(fgColor);
        lexer->setFoldComments(true);
        //lexer->setFoldPreprocessor(true);
        //lexer->setFoldAtElse(true);
        lexer->setFoldCompact(false);
        // -- css
        //lexer->setHighlightTripleQuotedStrings(true);
        //lexer->setHighlightHashQuotedStrings(true);
        //lexer->setHighlightBackQuotedStrings(true);
        lexer->setColor(fgColor, QsciLexerCSS::Default);
        lexer->setPaper(bgColor, QsciLexerCSS::Default);
        lexer->setColor(keyword1, QsciLexerCSS::Tag); 
        lexer->setColor(keyword2, QsciLexerCSS::ClassSelector);
        lexer->setColor(keyword2, QsciLexerCSS::PseudoClass);
        lexer->setColor(keyword2, QsciLexerCSS::UnknownPseudoClass);
        lexer->setColor(operatorColor, QsciLexerCSS::Operator);       
        lexer->setColor(keyword2, QsciLexerCSS::CSS1Property);
        lexer->setColor(keyword2, QsciLexerCSS::UnknownProperty);
        lexer->setColor(numberColor, QsciLexerCSS::Value); 
        lexer->setPaper(numberPaperColor, QsciLexerCSS::Value);
        lexer->setColor(commentColor, QsciLexerCSS::Comment); 
        lexer->setPaper(commentPaperColor, QsciLexerCSS::Comment);
        lexer->setColor(keyword2, QsciLexerCSS::IDSelector);
        lexer->setColor(keyword2, QsciLexerCSS::Important);
        lexer->setColor(keyword2, QsciLexerCSS::AtRule);
        lexer->setColor(stringColor, QsciLexerCSS::DoubleQuotedString); 
        lexer->setPaper(stringPaperColor, QsciLexerCSS::DoubleQuotedString); 
        lexer->setColor(keyword2, QsciLexerCSS::CSS2Property);
        lexer->setColor(keyword2, QsciLexerCSS::Attribute);
        lexer->setColor(keyword2, QsciLexerCSS::CSS3Property);
        lexer->setColor(keyword2, QsciLexerCSS::PseudoElement);
        lexer->setColor(keyword2, QsciLexerCSS::ExtendedCSSProperty);
        lexer->setColor(keyword2, QsciLexerCSS::ExtendedPseudoClass);
        lexer->setColor(keyword2, QsciLexerCSS::ExtendedPseudoElement);
        lexer->setColor(stringColor, QsciLexerCSS::MediaRule);
        lexer->setPaper(stringPaperColor, QsciLexerCSS::MediaRule); 
        lexer->setColor(fgColor, QsciLexerCSS::Variable);
        // -- 
        editor->setLexer(lexer);
        return true;
    } else if ( FILE_EXT_LEXER_JSON.contains(ext) ) {
        QsciLexerJSON* lexer = new QsciLexerJSON(editor);
        lexer->setDefaultPaper(bgColor);
        lexer->setDefaultColor(fgColor);
        //lexer->setFoldComments(true);
        //lexer->setFoldPreprocessor(true);
        //lexer->setFoldAtElse(true);
        lexer->setFoldCompact(false);
        // json 
        lexer->setColor(fgColor, QsciLexerJSON::Default);
        lexer->setColor(numberColor, QsciLexerJSON::Number);
        lexer->setPaper(numberPaperColor, QsciLexerJSON::Number);
        lexer->setColor(stringColor, QsciLexerJSON::String);
        lexer->setPaper(stringPaperColor, QsciLexerJSON::String);
        lexer->setColor(stringColor, QsciLexerJSON::UnclosedString);
        lexer->setPaper(stringPaperColor, QsciLexerJSON::UnclosedString);
        lexer->setColor(fgColor, QsciLexerJSON::Property);
        lexer->setColor(fgColor, QsciLexerJSON::EscapeSequence);
        lexer->setColor(commentColor, QsciLexerJSON::CommentLine);
        lexer->setPaper(commentPaperColor, QsciLexerJSON::CommentLine);
        lexer->setColor(commentColor, QsciLexerJSON::CommentBlock);
        lexer->setPaper(commentPaperColor, QsciLexerJSON::CommentBlock);
        lexer->setColor(operatorColor, QsciLexerJSON::Operator);
        lexer->setColor(fgColor, QsciLexerJSON::IRI);
        lexer->setColor(fgColor, QsciLexerJSON::IRICompact);
        lexer->setColor(keyword1, QsciLexerJSON::Keyword);
        lexer->setColor(keyword2, QsciLexerJSON::KeywordLD);
        lexer->setColor(fgColor, QsciLexerJSON::Error);
        //
        editor->setLexer(lexer);
        return true;
    } else if ( FILE_EXT_LEXER_SHELL.contains(ext) ) {
        QsciLexerBash* lexer = new QsciLexerBash(editor);
        lexer->setDefaultPaper(bgColor);
        lexer->setDefaultColor(fgColor);
        lexer->setFoldComments(true);
        //lexer->setFoldPreprocessor(true);
        //lexer->setFoldAtElse(true);
        lexer->setFoldCompact(false);
        // -- Bash style
        // Base colors
        lexer->setColor(fgColor, QsciLexerBash::Default);
        lexer->setPaper(bgColor, QsciLexerBash::Default);
        // Comments
        lexer->setColor(commentColor, QsciLexerBash::Comment);
        lexer->setPaper(commentPaperColor, QsciLexerBash::Comment);
        // Numbers and Keywords
        lexer->setColor(numberColor, QsciLexerBash::Number);
        lexer->setColor(keyword1, QsciLexerBash::Keyword);
        // Strings
        lexer->setColor(stringColor, QsciLexerBash::DoubleQuotedString);
        lexer->setPaper(stringPaperColor, QsciLexerBash::DoubleQuotedString);
        lexer->setColor(stringColor, QsciLexerBash::SingleQuotedString);
        lexer->setPaper(stringPaperColor, QsciLexerBash::SingleQuotedString);
        // Operators
        lexer->setColor(operatorColor, QsciLexerBash::Operator);
        // Variables (mapped to Identifier/Scalar)
        // In Bash, variables like $VAR are "Scalars"
        lexer->setColor(fgColor, QsciLexerBash::Identifier);
        lexer->setColor(preprocessorColor, QsciLexerBash::Scalar); 
        // Special Bash Features
        lexer->setColor(stringColor, QsciLexerBash::Backticks); // `command`
        lexer->setColor(keyword2, QsciLexerBash::ParameterExpansion); // ${VAR}
        // Errors
        lexer->setColor(stringColor, QsciLexerBash::Error);
        // 
        editor->setLexer(lexer);
        return true;
    } else if ( FILE_EXT_LEXER_BATCH.contains(ext) ) {
        QsciLexerBatch* lexer = new QsciLexerBatch(editor);
        lexer->setDefaultPaper(bgColor);
        lexer->setDefaultColor(fgColor);
        //lexer->setFoldComments(true);
        //lexer->setFoldPreprocessor(true);
        //lexer->setFoldAtElse(true);
        //lexer->setFoldCompact(false);
        // -- batch style
        lexer->setColor(fgColor, QsciLexerBatch::Default);
        lexer->setColor(commentColor, QsciLexerBatch::Comment);
        lexer->setPaper(commentPaperColor, QsciLexerBatch::Comment);
        lexer->setColor(keyword1, QsciLexerBatch::Keyword);
        lexer->setColor(stringColor, QsciLexerBatch::Label);
        lexer->setPaper(stringPaperColor, QsciLexerBatch::Label);
        lexer->setColor(stringColor, QsciLexerBatch::HideCommandChar);
        lexer->setPaper(stringPaperColor, QsciLexerBatch::HideCommandChar);
        lexer->setColor(operatorColor, QsciLexerBatch::Operator);
        lexer->setColor(classColor, QsciLexerBatch::Variable);
        lexer->setColor(functionColor, QsciLexerBatch::ExternalCommand);
        //
        editor->setLexer(lexer);
        return true;
    } else if ( FILE_EXT_LEXER_SQL.contains(ext) ) {
        QsciLexerSQL* lexer = new QsciLexerSQL(editor);
        lexer->setDefaultPaper(bgColor);
        lexer->setDefaultColor(fgColor);
        lexer->setFoldComments(true);
        //lexer->setFoldPreprocessor(true);
        lexer->setFoldAtElse(true);
        lexer->setFoldCompact(false);
        editor->setLexer(lexer);
        return true;
    } else if ( FILE_EXT_LEXER_PHP.contains(ext) ) {
        QsciLexerHTML* lexer = new QsciLexerHTML(editor);
        lexer->setDefaultPaper(bgColor);
        lexer->setDefaultColor(fgColor);
        //lexer->setFoldComments(true);
        lexer->setFoldPreprocessor(true);
        //lexer->setFoldAtElse(true);
        lexer->setFoldCompact(false);
        editor->setLexer(lexer);
        return true;
    } else if ( FILE_EXT_LEXER_RUBY.contains(ext) ) {
        QsciLexerRuby* lexer = new QsciLexerRuby(editor);
        lexer->setDefaultPaper(bgColor);
        lexer->setDefaultColor(fgColor);
        lexer->setFoldComments(true);
        //lexer->setFoldPreprocessor(true);
        //lexer->setFoldAtElse(true);
        lexer->setFoldCompact(false);
        editor->setLexer(lexer);
        return true;
    } else if ( FILE_EXT_LEXER_PERL.contains(ext) ) {
        QsciLexerPerl* lexer = new QsciLexerPerl(editor);
        lexer->setDefaultPaper(bgColor);
        lexer->setDefaultColor(fgColor);
        lexer->setFoldComments(true);
        //lexer->setFoldPreprocessor(true);
        lexer->setFoldAtElse(true);
        lexer->setFoldCompact(false);
        editor->setLexer(lexer);
        return true;
    } else if ( FILE_EXT_LEXER_LUA.contains(ext) ) {
        QsciLexerLua* lexer = new QsciLexerLua(editor);
        lexer->setDefaultPaper(bgColor);
        lexer->setDefaultColor(fgColor);
        //lexer->setFoldComments(true);
        //lexer->setFoldPreprocessor(true);
        //lexer->setFoldAtElse(true);
        lexer->setFoldCompact(false);
        editor->setLexer(lexer);
        return true;
    } else if ( FILE_EXT_LEXER_MARKDOWN.contains(ext) ) {
        QsciLexerMarkdown* lexer = new QsciLexerMarkdown(editor);
        lexer->setDefaultPaper(bgColor);
        lexer->setDefaultColor(fgColor);
        //lexer->setFoldComments(true);
        //lexer->setFoldPreprocessor(true);
        //lexer->setFoldAtElse(true);
        //lexer->setFoldCompact(false);
        editor->setLexer(lexer);
        return true;
    } else if ( FILE_EXT_LEXER_YAML.contains(ext) ) {
        QsciLexerYAML* lexer = new QsciLexerYAML(editor);
        lexer->setDefaultPaper(bgColor);
        lexer->setDefaultColor(fgColor);
        lexer->setFoldComments(true);
        //lexer->setFoldPreprocessor(true);
        //lexer->setFoldAtElse(true);
        //lexer->setFoldCompact(false);
        editor->setLexer(lexer);
        return true;
    } else if ( FILE_EXT_LEXER_INI.contains(ext) ) {
        QsciLexerProperties* lexer = new QsciLexerProperties(editor);
        lexer->setDefaultPaper(bgColor);
        lexer->setDefaultColor(fgColor);
        //lexer->setFoldComments(true);
        //lexer->setFoldPreprocessor(true);
        //lexer->setFoldAtElse(true);
        lexer->setFoldCompact(false);
        editor->setLexer(lexer);
        return true;
    } else if ( FILE_EXT_LEXER_CS.contains(ext) ) {
        QsciLexerCPP* lexer = new QsciLexerCSharp(editor);
        lexer->setDefaultPaper(bgColor);
        lexer->setDefaultColor(fgColor);
        lexer->setFoldComments(true);
        lexer->setFoldPreprocessor(true);
        lexer->setFoldAtElse(true);
        lexer->setFoldCompact(false);
        applyCPPStyle(lexer);
        editor->setLexer(lexer);
        return true;
    } else { // Default Lexer 
        QsciLexerCPP* lexer = new QsciLexerCPP(editor);
        lexer->setDefaultPaper(bgColor);
        lexer->setDefaultColor(fgColor);
        lexer->setFoldComments(true);
        lexer->setFoldPreprocessor(true);
        lexer->setFoldAtElse(true);
        lexer->setFoldCompact(false);
        applyCPPStyle(lexer);
        editor->setLexer(lexer);
        return true;
    }
    
}
bool CodexIncantation::setLexerFolding(QsciScintilla* editor, QString fileName) { // INCOMPLETE
    if(!editor) return false;
    QString ext = getExtension(fileName);
    // -- 
    editor->setFolding(QsciScintilla::BoxedTreeFoldStyle);
    editor->setFoldMarginColors(QColor("#21252b"), QColor("#21252b"));
    editor->setMarkerForegroundColor(fgColor);
    auto toBGR = [](QColor c) -> long { return (c.blue() << 16) | (c.green() << 8) | c.red(); };
    long bgrBack = toBGR(bgColor);
    long bgrFore = toBGR(fgColor);
    
    for (int i = 25; i <= 31; ++i) { // Range 25-31 covers all standard folding/outlining symbols
        editor->SendScintilla(QsciScintilla::SCI_MARKERSETBACK, i, bgrBack);
        editor->SendScintilla(QsciScintilla::SCI_MARKERSETFORE, i, bgrFore);
        // OPTIONAL: If using BoxedTree, you can also set the 'plus' sign color specifically
        // by making sure the marker is actually drawn as a box
        editor->SendScintilla(QsciScintilla::SCI_MARKERSETBACKSELECTED, i, bgrFore);
    }  
    return true;
}
void CodexIncantation::setAutocompletion(QsciScintilla* editor) {
    if (!editor || !editor->lexer()) return;
    if (editor->lexer()->apis()) return;
    // Link APIs to the existing lexer
    QsciAPIs* apis = new QsciAPIs(editor->lexer());
    // Configure Scintilla behavior
    editor->setAutoCompletionSource(QsciScintilla::AcsAPIs);
    editor->setAutoCompletionThreshold(2);
    editor->setAutoCompletionCaseSensitivity(false);
    editor->setAutoCompletionReplaceWord(true);
}
void CodexIncantation::updateAutocompletion_Range(QsciScintilla* editor) {
    int range = 1;
    QsciLexer* lexer = editor->lexer();
    if (!lexer || !editor) return;      
    QsciAPIs* apis = dynamic_cast<QsciAPIs*>(lexer->apis());
    if (!apis) return;
    // 1. Calculate line range
    int currentLine, index;
    editor->getCursorPosition(&currentLine, &index);
    int startLine = std::max(0, currentLine - range);
    int endLine = std::min(editor->lines() - 1, currentLine + range);
    // 2. Extract text (using Scintilla range is faster than editor->text().mid)
    int startPos = editor->SendScintilla(QsciScintilla::SCI_POSITIONFROMLINE, startLine);
    int endPos = editor->SendScintilla(QsciScintilla::SCI_GETLINEENDPOSITION, endLine);
    QString rangeText = editor->text(startPos, endPos); // Qt-native way to get range
    // 3. Get the existing word set from cache
    QHash<QString, QVariant>& data = SCINTILLA_DATA[editor]; 
    QSet<QString> knownWords = data.value("knownWords").value<QSet<QString>>();
    // 4. Tokenize and Add
    QRegularExpression re("\\b[A-Za-z_]\\w{2,}\\b");
    QRegularExpressionMatchIterator i = re.globalMatch(rangeText);
    QString currentWord = CodexIncantation::getCurrentTypingWord(editor);
    bool addedAny = false;
    while (i.hasNext()) {
        QString word = i.next().captured();
        if (knownWords.contains(word)) continue;
        if (word==currentWord) continue;
        knownWords.insert(word);
        apis->add(word);
        addedAny = true;
    }
    // 5. Finalize
    if (addedAny) {
        // Save the updated set back to the QVariant hash
        data.insert("knownWords", QVariant::fromValue(knownWords));
        apis->prepare();
    }
}
void CodexIncantation::updateAutocompletion_Full(QsciScintilla* editor) {
    QsciLexer* lexer = editor->lexer();
    if (!lexer || !editor) return;      
    QsciAPIs* apis = dynamic_cast<QsciAPIs*>(lexer->apis());
    if (!apis) return;

    // 1. Get current cache
    QHash<QString, QVariant>& data = SCINTILLA_DATA[editor]; 
    QSet<QString> knownWords = data.value("knownWords").value<QSet<QString>>();

    // 2. Scan text
    // Optimization: If file is huge, consider scanning in chunks 
    // but for now, we use the full text.
    QString fullText = editor->text();
    QRegularExpression re("\\b[A-Za-z_]\\w{2,}\\b");
    QRegularExpressionMatchIterator i = re.globalMatch(fullText);

    bool addedAny = false;
    while (i.hasNext()) {
        QString word = i.next().captured();
        if (!knownWords.contains(word)) {
            knownWords.insert(word);
            apis->add(word); // Only adding genuinely NEW words
            addedAny = true;
        }
    }

    // 3. Finalize
    if (addedAny) {
        // DO NOT call apis->clear() here or you lose the work above!
        data.insert("knownWords", QVariant::fromValue(knownWords));
        apis->prepare();
    }
}
QString CodexIncantation::getCurrentTypingWord(QsciScintilla* editor) {
    int line, index;
    editor->getCursorPosition(&line, &index);
    return editor->wordAtLineIndex(line, index);
}
void CodexIncantation::applyIndicatorsFromTextDirectives(QsciScintilla* editor) {
    if (!editor) return;
    clearIndicators(editor);
    int maxLine = std::min(100, editor->lines());
    int indicatorId = 8; 
    for (int i = 0; i < maxLine; ++i) {
        QString lineText = editor->text(i);
        QRegularExpressionMatch match = directiveRegex.match(lineText);
        if (match.hasMatch()) {
            QStringList groups = match.captured(1).split('|', Qt::SkipEmptyParts);
            for (const QString& group : groups) {
                QStringList parts = group.split(':');
                if (parts.size() < 2 || indicatorId > 31) continue;
                QColor color(parts[0].trimmed());
                QStringList tokens = parts[1].split(',', Qt::SkipEmptyParts);
                // Setup Indicator
                setIndicator(editor, indicatorId, color);
                for (const QString& token : tokens) {
                    QString cleanToken = token.trimmed();
                    if (cleanToken.isEmpty()) continue;
                    // --- TARGET SEARCH LOGIC (The Fix) ---
                    QByteArray bytes = cleanToken.toUtf8();
                    int docLength = editor->SendScintilla(QsciScintilla::SCI_GETLENGTH);
                    // Reset search range to full document for every new token
                    editor->SendScintilla(QsciScintilla::SCI_SETTARGETSTART, 0);
                    editor->SendScintilla(QsciScintilla::SCI_SETTARGETEND, docLength);
                    // Match whole words only (2) | Case sensitive (0 or 4)
                    editor->SendScintilla(QsciScintilla::SCI_SETSEARCHFLAGS, 2); 
                    while (editor->SendScintilla(QsciScintilla::SCI_SEARCHINTARGET, bytes.length(), bytes.constData()) != -1) {
                        int mStart = editor->SendScintilla(QsciScintilla::SCI_GETTARGETSTART);
                        int mEnd = editor->SendScintilla(QsciScintilla::SCI_GETTARGETEND);
                        // Apply indicator using raw positions
                        editor->SendScintilla(QsciScintilla::SCI_INDICATORFILLRANGE, mStart, mEnd - mStart);
                        // Move search target past the current match
                        editor->SendScintilla(QsciScintilla::SCI_SETTARGETSTART, mEnd);
                        editor->SendScintilla(QsciScintilla::SCI_SETTARGETEND, docLength);
                    }
                }
                indicatorId++; 
            }
        }
    }
}

// Incantation || namespace TabbedSplitView 
QsciScintilla* TabbedSplitView::addLeftTab_Scintilla(QSplitter* view, QString name) {
    QsciScintilla* result = CodexIncantation::newDarkScintilla(view);
    TabbedSplitView::addLeftTab(view, name, *result);
    return result;
}
QsciScintilla* TabbedSplitView::addRightTab_Scintilla(QSplitter* view, QString name) {
    QsciScintilla* result = CodexIncantation::newDarkScintilla(view);
    TabbedSplitView::addRightTab(view, name, *result);
    return result;
}
QsciScintilla* TabbedSplitView::dialogScintillaTabLoad(QTabWidget* tabs) {
    if (!tabs) return nullptr;
    QSplitter* splitter = findClosestParent<QSplitter>(tabs);
    if (!splitter) return nullptr;
    QString fileName = QFileDialog::getOpenFileName(tabs);
    if ( fileName.isNull() || fileName.isEmpty() ) return nullptr;
    if (TabbedSplitView::isFileAlreadyOpened(tabs,fileName)) { 
        QMessageBox::warning(tabs, "Aborted", "File is Already Opened");
        return nullptr; 
    }
    QString new_tab_text = getShortFileName(fileName);
    if (new_tab_text.isNull() || new_tab_text.isEmpty()) return nullptr;
    QsciScintilla* editor = CodexIncantation::newDarkScintilla(splitter, fileName); // work-around
    if (!editor) return nullptr;
    int tabIndex = tabs->addTab(editor, QString("[ ")+new_tab_text+QString(" ]"));
    if (tabIndex == -1) return nullptr;    
    tabs->setTabText(tabIndex, QString("[ ")+new_tab_text+QString(" ]"));
    tabs->tabBar()->setTabData(tabIndex, QVariant(fileName));
    tabs->setTabToolTip(tabIndex, fileName); // ISSUE
    QString content = loadFile(fileName);
    editor->blockSignals(true);
    editor->setText(content);
    editor->blockSignals(false);
    CodexIncantation::updateAutocompletion_Full(editor);
    editor->setReadOnly(true);
    editor->foldAll(true);
    editor->setFocus();
    return editor;
}
QsciScintilla* TabbedSplitView::loadScintillaFromFilename(QTabWidget* tabs, QString fileName) {
    if (!tabs) return nullptr;
    QSplitter* splitter = findClosestParent<QSplitter>(tabs);
    if (!splitter) return nullptr;
    fileName = fileExists(fileName);
    if (fileName.isEmpty() || fileName.isNull()) return nullptr;
    if (TabbedSplitView::isFileAlreadyOpened(tabs,fileName)) { 
        QMessageBox::warning(tabs, "Aborted", "File is Already Opened");
        return nullptr; 
    }
    QString new_tab_text = getShortFileName(fileName);
    if (new_tab_text.isNull() || new_tab_text.isEmpty()) return nullptr;
    QsciScintilla* editor = CodexIncantation::newDarkScintilla(splitter, fileName); // work-around
    if (!editor) return nullptr;
    int tabIndex = tabs->addTab(editor, QString("[ ")+new_tab_text+QString(" ]"));
    if (tabIndex == -1) return nullptr;    
    tabs->tabBar()->setTabData(tabIndex, QVariant(fileName));
    tabs->setTabToolTip(tabIndex, fileName); // ISSUE
    QString content = loadFile(fileName);
    editor->blockSignals(true);
    editor->setText(content);
    editor->blockSignals(false); 
    CodexIncantation::updateAutocompletion_Full(editor);
    editor->setReadOnly(true);
    editor->foldAll(true);
    return editor;
}
bool TabbedSplitView::isFileAlreadyOpened(QSplitter* splitter, QString absFilepath) {
    if (absFilepath.isNull() || absFilepath.isEmpty()) return false;
    QTabWidget* ltabs = TabbedSplitView::getTabsByName(splitter,"leftTabs");
    QTabWidget* rtabs = TabbedSplitView::getTabsByName(splitter,"rightTabs");
    if(!ltabs || !rtabs) return false; 
    // QMessageBox::warning(nullptr, "Debug", "here");
    int fresult = -1;
    fresult = TabbedSplitView::foreachTab(ltabs, 
        [absFilepath](int tabIndex, QWidget* widget, QTabWidget* tabs)->int {
            if (!widget) return 0;
            QVariant data = tabs->tabBar()->tabData(tabIndex);
            if (!data.isValid() || data.isNull()) return 0;
            QString data_stored_path = data.value<QString>();
            if (data_stored_path == absFilepath) return 1;
            return 0;
        }
    );
    if (fresult == 1) return true;
    fresult = TabbedSplitView::foreachTab(rtabs, 
        [absFilepath](int tabIndex, QWidget* widget, QTabWidget* tabs)->int {
            if (!widget) return 0;
            QVariant data = tabs->tabBar()->tabData(tabIndex);
            if (!data.isValid() || data.isNull()) return 0;
            QString data_stored_path = data.value<QString>();
            if (data_stored_path == absFilepath) return 1;
            return 0;
        }
    );
    if (fresult == 1) return true;
    return false;
}
bool TabbedSplitView::isFileAlreadyOpened(QTabWidget* currentTabs, QString absFilepath) {
    QSplitter* splitter = CodexIncantation::findClosestParent<QSplitter>(currentTabs);
    if (!splitter) return false; 
    return TabbedSplitView::isFileAlreadyOpened(splitter,absFilepath);
}



// -- END 

