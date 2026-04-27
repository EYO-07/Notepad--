// {TextMarker|cyan:>>>,<<<,TODO|red:ISSUE|yellow:TESTING,INCOMPLETE,DEPRECATED|silver:SCULPT,UNUSED|magenta:LOGICAL_ISSUE}
// {TextMarker|blue:}
// {Search:}
// -- BEGIN 
// GOLEM main.cpp 

#include <QCoreApplication>
#include <QApplication>
#include "CODEX_DarkQt.h"
#include "CODEX_DarkQtScintilla.h"
using namespace CodexTransmutation;
using namespace CodexIncantation;

// global variables
QString currentSearchString("");
QString lastSearchString("");
CodexIncantation::FileRegistry FILE_REGISTRY("Notepad__LinuxQtSharedFiles");
// QFileSystemWatcher FILE_WATCHER; 
// QHash<QString, QDateTime> FILE_MODIFICATION;
// forward declaration
void darkTabScintillaLogic(QsciScintilla* view);
QsciScintilla* addLeftTab_Scintilla(QSplitter* view, QString name);
QsciScintilla* addRightTab_Scintilla(QSplitter* view, QString name);
bool newEmptyScintillaTab(QsciScintilla* view);
void log(QString line, QsciScintilla* editor);
void logError(QString line, QsciScintilla* editor);
void logGreen(QString line, QsciScintilla* editor);

// local classes 

// local functions 
void debugFileWatcher(const QFileSystemWatcher& watcher) { // UNUSED 
    QString filePaths("");
    for(const auto& item: watcher.files()) {
        filePaths+item+",";
    }
    QMessageBox::information(nullptr, "Files Being Watched", filePaths);
}

// -- 
int main(int argc, char *argv[]) {
    // FILE_MODIFICATION = QHash<QString, QDateTime>();
    // Components    
    QApplication app(argc, argv);    
    QMainWindow* window = new QMainWindow();
    QSplitter* splitter = TabbedSplitView::tabbedSplitView(window);
    QTabWidget* ltabs = TabbedSplitView::getTabsByName(splitter, "leftTabs");
    QTabWidget* rtabs = TabbedSplitView::getTabsByName(splitter, "rightTabs");
    QsciScintilla* helpPage = addLeftTab_Scintilla(splitter, "?"); 
    QsciScintilla* clipboardPage = addRightTab_Scintilla(splitter, "..."); 
    helpPage->setText(
        "// Notepad--LinuxQt : linux version of Notepad-- using Qt6 QScintilla \n"
        "// ... lesser version of Notepad++, less language support, fixed dark theme. \n"
        "// ... it's bit different from Notepad-- too ... \n"
        "// ... don't like it? use Notepadqq instead (or Nano, or Geany, or Vim ...) \n"
        "\n"
        "// Text Marker Document Directive Syntax {TextMarker|yellow:editor}\n"
        "// Search Document Directive Syntax {Search:token}"
        "\n"
        "1. [ TabTitle ] // ReadOnly tab, use Ctrl+R to change ReadOnly flag, by default the file opens on readonly mode \n"
        "2. * TabTitle // Is the editor text modified flag \n"
        "\n"
        "// Commands \n"
        "1. Ctrl+R 'change readonly flag so you can actually edit' \n"
        "2. Ctrl+L // load file \n"
        "3. Ctrl+S // save file and update the Text Markers \n"
        "4. Ctrl+W // close tab \n"
        "5. Ctrl+M // create a new empty file. (you still need to load it using Ctrl+L) \n"
        "\n"
        "1. Ctrl+Tab, Ctrl+Shift+Tab // Change Selected Tab \n"
        "\n"
        "1. Alt+V // Switch view for current tab \n"
        "2. Alt+Left, Alt+Right // Focus the Left or Right current tab \n"
        "3. Alt+S // Change the split orientation\n"
        "4. Alt+A, Alt+D // Move the splitter separator\n"
        "\n"
        "1. Ctrl+F, Ctrl+D // Find Next, Find Previous Text, will prompt a input dialog if don't find search directive on document. \n"
        "\n"
        "1. F1 // Take Screenshot of the Current Editor to clipboard\n"
        "2. F2 // Take Screenshot of the Current Editor to file \n"
        "\n"
        "... use this tabpage as you wish, its not stored anywhere. \n"
    );
    clipboardPage->setText(
        "// Clipboard | Log\n"
        "... use this tabpage as you wish, its not stored anywhere. \n\n"
    );
    helpPage->setFocus();
    log("Notepad-- Started",clipboardPage);
    { // Layout 
        // load files by argument options
        if (argc>1) {
            log("Processing Command Line Arguments",clipboardPage);
            int i;
            bool b_left_side = true;
            for (int i = 1; i < argc; ++i) {
                QString strArgument = argv[i];
                if (strArgument=="--left") {
                    b_left_side = true;
                    continue;
                } 
                if (strArgument=="--right") {
                    b_left_side = false;
                    continue;
                }
                QString absDirPath = isDir(strArgument);
                if (!absDirPath.isEmpty() && !absDirPath.isNull()) { 
                    logError("Failed to Load File",clipboardPage);
                    log(QString("The file '%1' is a directory" ).arg(absDirPath),clipboardPage);
                    continue; 
                }
                QString absFileName = fileExists(strArgument);
                if (absFileName.isEmpty() || absFileName.isNull()) {
                    logError(
                        QString("Failed to Load File : Invalid Filename (%1)").arg(strArgument),
                        clipboardPage
                    );
                    continue;
                }
                QTabWidget* tabs = nullptr;
                if (b_left_side) {
                    tabs = TabbedSplitView::getTabsByName(splitter, "leftTabs");
                } else {
                    tabs = TabbedSplitView::getTabsByName(splitter, "rightTabs");
                }
                QsciScintilla* editor = TabbedSplitView::loadScintillaFromFilename(tabs, absFileName);
                if (!editor) { 
                    logError("Failed to Load File : is the file opened in another instance ?",clipboardPage);
                    continue;
                }
                darkTabScintillaLogic(editor);
                QFileInfo info(absFileName);
                // FILE_WATCHER.addPath(info.absoluteFilePath()); // TESTING ISSUE addPath don't work 
                log(QString("File Loaded by Command Line Argument // %1").arg(absFileName), clipboardPage);
            }
        } 
    }
    { // Theme
        CodexIncantation::applyDarkTheme(window);
    }
    { // Logic 
        CodexIncantation::interceptKeyboardEvents(window, [splitter,ltabs,rtabs](QKeyEvent* e) -> bool {
            if (e->modifiers() == Qt::ControlModifier) {
                if (e->key() == Qt::Key_L) {
                    QsciScintilla* newEditor = TabbedSplitView::dialogScintillaTabLoad(ltabs);
                    if (!newEditor) return true;
                    darkTabScintillaLogic(newEditor);
                    return true;
                }
            } else if (e->modifiers() == Qt::AltModifier) {
                if (e->key() == Qt::Key_Left) {
                    int l_currentTab = ltabs->currentIndex();
                    if (l_currentTab==-1) return true;
                    QWidget* lwdg = ltabs->widget(l_currentTab);
                    if (!lwdg) return true;
                    lwdg->setFocus();
                    return true;
                }
                if (e->key() == Qt::Key_Right) {
                    int r_currentTab = rtabs->currentIndex();
                    if (r_currentTab==-1) return true;
                    QWidget* rwdg = rtabs->widget(r_currentTab);
                    if (!rwdg) return true;
                    rwdg->setFocus();
                    return true;
                }
            }
            return false;
        });
        CodexIncantation::interceptKeyboardEvents(ltabs, [splitter,ltabs,rtabs](QKeyEvent* e) -> bool {
            if (e->modifiers() == Qt::AltModifier) {
                if (e->key() == Qt::Key_Left) {
                    int l_currentTab = ltabs->currentIndex();
                    if (l_currentTab==-1) return true;
                    QWidget* lwdg = ltabs->widget(l_currentTab);
                    if (!lwdg) return true;
                    lwdg->setFocus();
                    return true;
                }
                if (e->key() == Qt::Key_Right) {
                    int r_currentTab = rtabs->currentIndex();
                    if (r_currentTab==-1) return true;
                    QWidget* rwdg = rtabs->widget(r_currentTab);
                    if (!rwdg) return true;
                    rwdg->setFocus();
                    return true;
                }
            }
            return false;
        });
        CodexIncantation::interceptKeyboardEvents(rtabs, [splitter,ltabs,rtabs](QKeyEvent* e) -> bool {
            if (e->modifiers() == Qt::AltModifier) {
                if (e->key() == Qt::Key_Left) {
                    int l_currentTab = ltabs->currentIndex();
                    if (l_currentTab==-1) return true;
                    QWidget* lwdg = ltabs->widget(l_currentTab);
                    if (!lwdg) return true;
                    lwdg->setFocus();
                    return true;
                }
                if (e->key() == Qt::Key_Right) {
                    int r_currentTab = rtabs->currentIndex();
                    if (r_currentTab==-1) return true;
                    QWidget* rwdg = rtabs->widget(r_currentTab);
                    if (!rwdg) return true;
                    rwdg->setFocus();
                    return true;
                }
            }
            return false;
        });
        CodexIncantation::interceptKeyboardEvents(splitter, [splitter,ltabs,rtabs](QKeyEvent* e) -> bool {
            if (e->modifiers() == Qt::AltModifier) {
                if (e->key() == Qt::Key_Left) {
                    int l_currentTab = ltabs->currentIndex();
                    if (l_currentTab==-1) return true;
                    QWidget* lwdg = ltabs->widget(l_currentTab);
                    if (!lwdg) return true;
                    lwdg->setFocus();
                    return true;
                }
                if (e->key() == Qt::Key_Right) {
                    int r_currentTab = rtabs->currentIndex();
                    if (r_currentTab==-1) return true;
                    QWidget* rwdg = rtabs->widget(r_currentTab);
                    if (!rwdg) return true;
                    rwdg->setFocus();
                    return true;
                }
            }
            return false;
        });
        
        /* Dialetic 
        - Which parts of the project should be modified to autodetect external file change ?
        : 1. Loading Logic || Should register the file to be watched 
        : 2. Saving Logic || Should prevent the logic to be processed, only external files
        : 3. Closing Tab Logic || Should remove the associated file 
        -- How to block the modification detection by the program itself ?
        - What the program should do whenever a file is externally modified ?
        -- { The file don't exist anymore } ?
        -- { The file was renamed } ?
        -- { The file was changed in content } ? 
        - What this callback should do if the file was externally modified ?
        -- < Silent Reload > ? 
        -- < Silent Reload with a logging in _journal.h > ?
        -- < Dialog Reload > ? 
        -- < Automatic Closing if the file don't exist anymore > ?
        */
        
        /*
        QObject::connect(&FILE_WATCHER, &QFileSystemWatcher::fileChanged, // TODO LOGICAL_ISSUE 
            [clipboardPage](const QString &path){ 
                
                log(QString("File Changed : %1").arg(path),clipboardPage);
            }
        ); 
        */
    }
    // -- 
    window->setCentralWidget(splitter);
    window->resize(1200, 800);
    window->show();
    auto result = app.exec();
    { // Clean-Up 
        // Clean-Up | Multi-instance Shared Memory Clean-up
        TabbedSplitView::foreachTab(rtabs, [](int idx,QWidget* wdg, QTabWidget* tabs) -> int {
            QString absFileName = TabbedSplitView::getScintillaFullFileName(tabs, idx);
            if (absFileName.isEmpty()) return 0;
            if (absFileName.isNull()) return 0;
            FILE_REGISTRY.unregisterFile(absFileName);
            return 0; 
        } );
        TabbedSplitView::foreachTab(ltabs, [](int idx,QWidget* wdg, QTabWidget* tabs) -> int {
            QString absFileName = TabbedSplitView::getScintillaFullFileName(tabs, idx);
            if (absFileName.isEmpty()) return 0;
            if (absFileName.isNull()) return 0;
            FILE_REGISTRY.unregisterFile(absFileName);
            return 0; 
        } );
    }
    return result;
}

// function implementation
void darkTabScintillaLogic(QsciScintilla* view) {
    CodexIncantation::interceptKeyboardEvents(view, [view](QKeyEvent* e) -> bool {
        QTabWidget* tabs = CodexIncantation::findClosestParent<QTabWidget>(view);
        if (!tabs) return true;
        int currentTab = tabs->currentIndex();
        if (currentTab == -1) return true;
        QString prev_tab_text = tabs->tabText(currentTab);
        // -- key processing 
        if (e->modifiers() == Qt::AltModifier) { // Alt
            if (e->key() == Qt::Key_O) {
                int line, col;
                view->getCursorPosition(&line, &col);
                view->SendScintilla(QsciScintilla::SCI_TOGGLEFOLD, line);
                return true; // "Eat" the event: Scintilla won't see it
            }
            if (e->key() == Qt::Key_0) {
                view->foldAll(true);
                return true; // Stop event propagation
            }
            if (e->key() == Qt::Key_A) {
                QSplitter* splitter = CodexIncantation::findClosestParent<QSplitter>(tabs);
                if (!splitter) return true;
                CodexIncantation::moveSeparator(splitter, -5);
                return true;
            }
            if (e->key() == Qt::Key_S) {
                QSplitter* splitter = CodexIncantation::findClosestParent<QSplitter>(tabs);
                if (!splitter) return true;
                CodexIncantation::toggleOrientation(splitter);
                return true;
            }
            if (e->key() == Qt::Key_D) {
                QSplitter* splitter = CodexIncantation::findClosestParent<QSplitter>(tabs);
                if (!splitter) return true;
                CodexIncantation::moveSeparator(splitter, 5);
                return true;
            }
            if (e->key() == Qt::Key_Up) {}
            if (e->key() == Qt::Key_Down) {}
            if (e->key() == Qt::Key_Left) {
                if ( tabs->objectName() == "leftTabs" ) return true;
                QSplitter* splitter = CodexIncantation::findClosestParent<QSplitter>(tabs);
                if (!splitter) return true;
                QTabWidget* left_tabs = splitter->findChild<QTabWidget*>("leftTabs");
                if (!left_tabs) return true;
                int l_currentTab = left_tabs->currentIndex();
                if (l_currentTab==-1) return true;
                QWidget* lwdg = left_tabs->widget(l_currentTab);
                if (!lwdg) return true;
                lwdg->setFocus();
                return true;
            }
            if (e->key() == Qt::Key_Right) {
                if ( tabs->objectName() == "rightTabs" ) return true;
                QSplitter* splitter = CodexIncantation::findClosestParent<QSplitter>(tabs);
                if (!splitter) return true;
                QTabWidget* right_tabs = splitter->findChild<QTabWidget*>("rightTabs");
                if (!right_tabs) return true;
                int r_currentTab = right_tabs->currentIndex();
                if (r_currentTab==-1) return true;
                QWidget* rwdg = right_tabs->widget(r_currentTab);
                if (!rwdg) return true;
                rwdg->setFocus();
                return true;
            }
            if (e->key() == Qt::Key_V) {
                QTabWidget* source = CodexIncantation::findClosestParent<QTabWidget>(view);
                QTabWidget* dest = nullptr;
                if (!source) return true;
                QSplitter* splitter = CodexIncantation::findClosestParent<QSplitter>(source);
                if (!splitter) return true;
                QString source_name = source->objectName();
                if (source_name == "leftTabs") {
                    dest = splitter->findChild<QTabWidget*>("rightTabs");
                } else if (source_name == "rightTabs") {
                    dest = splitter->findChild<QTabWidget*>("leftTabs");
                }
                if (!dest) return true;
                TabbedSplitView::switchTabs(currentTab,source,dest);
                return true;
            }
        } else if (e->modifiers() == Qt::ControlModifier) { // Control 
            if (e->key() == Qt::Key_L) { // TESTING LOGICAL_ISSUE 
                QsciScintilla* newEditor = TabbedSplitView::dialogScintillaTabLoad(tabs);
                if (!newEditor) return true;
                int newEditorIndex = tabs->indexOf(newEditor);
                if (newEditorIndex == -1) return true;
                QString newEditorFullFileName = TabbedSplitView::getScintillaFullFileName(tabs,newEditorIndex);
                // FILE_WATCHER.addPath(newEditorFullFileName); // ISSUE addPath don't work 
                darkTabScintillaLogic(newEditor);
                return true;
            }
            if (e->key() == Qt::Key_S) { 
                if (!tabs) return true;
                CodexIncantation::applyIndicatorsFromTextDirectives(view);
                QVariant data = tabs->tabBar()->tabData(currentTab);
                if(data.isNull() || !data.isValid()) return true;
                QString absPath = data.value<QString>();
                if ( !fileExists(absPath).isNull() && !fileExists(absPath).isEmpty() ) {
                    if ( view->isReadOnly() ) {
                        QMessageBox::warning(view, "Aborted", "The editor for "+absPath+" is read only");
                        return true;
                    }
                    saveFile(absPath, view->text());
                    QMessageBox::information(view, "File Saved", "File Saved on: "+absPath);
                    if ( prev_tab_text.startsWith("* ") ) {
                        tabs->setTabText(currentTab,prev_tab_text.sliced(2));
                    }
                } else {}
                return true;
            }
            if (e->key() == Qt::Key_N) { // UNUSED 
                return true; 
            }
            if (e->key() == Qt::Key_R) {
                if (!tabs) return true;
                QVariant data = tabs->tabBar()->tabData(currentTab);
                if(data.isNull() || !data.isValid()) return true;
                view->setReadOnly(!view->isReadOnly());
                QString absPath = data.value<QString>();
                QString fileName = getShortFileName(absPath);
                if (view->isReadOnly()) {
                    if ( isEnclosedBy(prev_tab_text, "[ ", " ]") ) return true;
                    tabs->setTabText(currentTab, QString("[ ")+prev_tab_text+QString(" ]"));
                } else {
                    if ( !isEnclosedBy(prev_tab_text, "[ ", " ]") ) return true;
                    int len = prev_tab_text.length();
                    tabs->setTabText(currentTab, prev_tab_text.sliced(2,len-4));
                }
                return true;
            }
            if (e->key() == Qt::Key_M) {
                CodexIncantation::createEmptyFileDialog();
                return true;
            }
            if (e->key() == Qt::Key_W) {
                QString absFileName = TabbedSplitView::getScintillaFullFileName(tabs, currentTab);
                FILE_REGISTRY.unregisterFile(absFileName);
                // FILE_WATCHER.removePath(absFileName); // TESTING
                TabbedSplitView::removeAndDestroyTab(tabs, currentTab);
                if (tabs->count()==0) return true;
                int tabIndex = tabs->currentIndex();
                if(tabIndex==-1) { 
                    tabs->setCurrentIndex(0);
                    tabIndex = tabs->currentIndex();
                }
                if(tabIndex==-1) return true; 
                QWidget* wdg = tabs->widget(tabIndex);
                if (!wdg) return true;
                wdg->setFocus();
                return true;
            }
            if (e->key() == Qt::Key_F) {
                // -> Ctrl+F || $currentSearchString | findNext()
                // -> Ctrl+F || $currentSearchString || getSearchStringFromDocDirective() | % didn find || input dialog 
                currentSearchString = CodexIncantation::getSearchStringFromDocDirective(view);
                if (currentSearchString.isNull() || currentSearchString.isEmpty()) {
                    bool ok;
                    lastSearchString = QInputDialog::getText(
                        view, 
                        "Find Next",
                        "Search Text :", 
                        QLineEdit::Normal,
                        lastSearchString, 
                        &ok
                    );
                    if (ok && !lastSearchString.isEmpty()) {
                        currentSearchString = lastSearchString;
                    } else {
                        return true;
                    }
                    CodexIncantation::findNext(view,currentSearchString);
                    return true;
                } else {
                    CodexIncantation::findNext(view,currentSearchString);
                    return true;
                }
            }
            if (e->key() == Qt::Key_D) {
                // -> Ctrl+D || $currentSearchString | findPrevious()
                // -> Ctrl+D || $currentSearchString || getSearchStringFromDocDirective() | % didn find || input dialog 
                currentSearchString = CodexIncantation::getSearchStringFromDocDirective(view);
                if (currentSearchString.isNull() || currentSearchString.isEmpty()) {
                    bool ok;
                    lastSearchString = QInputDialog::getText(
                        view, 
                        "Find Previous",
                        "Search Text :", 
                        QLineEdit::Normal,
                        lastSearchString, 
                        &ok
                    );
                    if (ok && !lastSearchString.isEmpty()) {
                        currentSearchString = lastSearchString;
                    } else {
                        return true;
                    }
                    CodexIncantation::findPrevious(view,currentSearchString);
                    return true;
                } else {
                    CodexIncantation::findPrevious(view,currentSearchString);
                    return true;
                }
            }
        } else if (e->key() == Qt::Key_F1) { // TESTING Screenshot to Clipboard
            CodexIncantation::takeWidgetScreenshot(view);
            return true;
        } else if (e->key() == Qt::Key_F2) { // Editor Screenshot to File 
            if ( 
                QMessageBox::No == QMessageBox::question(
                    view, 
                    "Editor Screenshot", 
                    "Are you sure to take screenshot from the current editor?", 
                    QMessageBox::Yes|QMessageBox::No
                )
            ) { return true; }
            QString fnm = TabbedSplitView::getScintillaFullFileName(tabs,currentTab);
            if ( fnm.isEmpty() || fnm.isNull() ) {
                bool ok = false;
                QString input = QInputDialog::getText(
                    view, 
                    "Editor Screenshot",
                    "Image Filename :", 
                    QLineEdit::Normal,
                    "", 
                    &ok
                );
                if (ok && !input.isEmpty() && !input.isNull()) {
                    fnm = input;
                } else {
                    return true;
                }
            }
            QString screenshotPath; {
                QString tkn = CodexTransmutation::getShortFileName(fnm);
                if (tkn.isNull() || tkn.isEmpty()) return true;
                tkn = tkn.replace('.','_');
                tkn = tkn.trimmed();
                tkn = tkn.replace(' ','_');
                using CodexTransmutation::joinPaths;
                QString exeDir = QCoreApplication::applicationDirPath();
                screenshotPath = joinPaths(exeDir,joinPaths("EditorScreenshots",tkn));
            }
            if (screenshotPath.isNull() || screenshotPath.isEmpty()) { 
                QMessageBox::critical(view, "Error", "Empty Path");
                return true; 
            }
            CodexIncantation::takeWidgetScreenshot(view,screenshotPath);
            return true;
        }
        return false; // Let all other keys (letters, arrows, etc.) pass to Scintilla
    });
    CodexIncantation::onTextChange(view, [](QsciScintilla* view) -> void {
        if ( !view ) return;
        if ( view->isReadOnly() ) return;
        QTabWidget* tabs = CodexIncantation::findClosestParent<QTabWidget>(view);
        if (!tabs) return;
        int currentTab = tabs->currentIndex();
        if (currentTab == -1) return;
        // update autocompletion words 
        CodexIncantation::updateAutocompletion_Range(view);
        // update tab label
        QVariant data = tabs->tabBar()->tabData(currentTab);
        if(data.isNull() || !data.isValid()) return;
        QString prev_tab_text = tabs->tabText(currentTab);
        if ( prev_tab_text.startsWith("* ") ) return;
        tabs->setTabText(currentTab,"* "+prev_tab_text);
    });
}
QsciScintilla* addLeftTab_Scintilla(QSplitter* view, QString name) {
    QsciScintilla* result = TabbedSplitView::addLeftTab_Scintilla(view,name);
    darkTabScintillaLogic(result);
    return result;
}
QsciScintilla* addRightTab_Scintilla(QSplitter* view, QString name) {
    QsciScintilla* result = TabbedSplitView::addRightTab_Scintilla(view,name);
    darkTabScintillaLogic(result);
    return result;
}
void log(QString line, QsciScintilla* editor) {
    QString timestamp = QDateTime::currentDateTime().toString(Qt::ISODate);
    QString textToAppend = "["+timestamp+"] "+line+"\n";
    editor->append( textToAppend );
}
void logError(QString line, QsciScintilla* editor) {
    QString timestamp = QDateTime::currentDateTime().toString(Qt::ISODate);
    QString textToAppend = "["+timestamp+"] '"+line+"'\n";
    editor->append( textToAppend );
}
void logGreen(QString line, QsciScintilla* editor) {
    QString timestamp = QDateTime::currentDateTime().toString(Qt::ISODate);
    QString textToAppend = "["+timestamp+"] //"+line+"\n";
    editor->append( textToAppend );
}

// -- END 

