// {TextMarker|cyan:>>>,<<<,TODO|red:ISSUE|yellow:INCOMPLETE,DEPRECATED|silver:SCULPT}
// {Search:}
// -- BEGIN 
// GOLEM main.cpp 

#include <QApplication>
#include <QPushButton>
#include <QWidget>
#include <QVBoxLayout> // Layout manager
#include <QTextEdit>
#include <QLabel>
#include <QtGlobal>
#include <iostream>
#include "CODEX_DarkQt.h"
#include "CODEX_DarkQtScintilla.h"
using namespace CodexTransmutation;
using namespace CodexIncantation;

// global variables
int index_new_tab = 1;
QString currentSearchString("");
QString lastSearchString("");
// functions forward declaration
void darkTabScintillaLogic(QsciScintilla* view);
QsciScintilla* addLeftTab_Scintilla(QSplitter* view, QString name);
QsciScintilla* addRightTab_Scintilla(QSplitter* view, QString name);
bool newEmptyScintillaTab(QsciScintilla* view);

// -- 
int main(int argc, char *argv[]) {
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
        "// Search Document Directive Syntax {Search:}"
        "\n"
        "1. [ TabTitle ] // ReadOnly tab, use Ctrl+R to change ReadOnly flag, by default the file opens on readonly mode \n"
        "2. * TabTitle // Is the editor text modified flag \n"
        "\n"
        "// Commands \n"
        "1. Ctrl+R // change readonly flag \n"
        "2. Ctrl+L // load file \n"
        "3. Ctrl+S // save file and update the Text Markers \n"
        "4. Ctrl+W // close tab \n"
        "\n"
        "1. Ctrl+Tab, Ctrl+Shift+Tab // Change Selected Tab from Current Tab Set \n"
        "\n"
        "1. Alt+V // Switch view for current tab \n"
        "2. Alt+Left, Alt+Right // Focus the Left or Right current tab \n"
        "3. Alt+S // Change the split orientation\n"
        "4. Alt+A, Alt+B // Move the splitter separator\n"
        "\n"
        "1. Ctrl+F, Ctrl+D // Find Next, Find Previous Text\n"
    );
    clipboardPage->setText(
        "// Draft | Clipboard | Log\n"
        "... use this tabpage as you wish"
    );
    helpPage->setFocus();
    { // Layout 
        // load files by argument options
        if (argc>1) {
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
                if (!absDirPath.isEmpty() && !absDirPath.isNull()) continue;
                QString absFileName = fileExists(strArgument);
                if (absFileName.isEmpty() || absFileName.isNull()) continue;
                QTabWidget* tabs = nullptr;
                if (b_left_side) {
                    tabs = TabbedSplitView::getTabsByName(splitter, "leftTabs");
                } else {
                    tabs = TabbedSplitView::getTabsByName(splitter, "rightTabs");
                }
                QsciScintilla* editor = TabbedSplitView::loadScintillaFromFilename(tabs, absFileName);
                if (!editor) continue;
                darkTabScintillaLogic(editor);
            }
        } else {
            //addLeftTab_Scintilla(splitter, QString("(%1)").arg(index_new_tab) );
            //index_new_tab++;
            //addRightTab_Scintilla(splitter, QString("(%1)").arg(index_new_tab) );
            //index_new_tab++;
        }
    }
    { // Theme
        CodexIncantation::applyDarkTheme(window);
    }
    { // Logic 
        CodexIncantation::interceptKeyboardEvents(window, [splitter,ltabs,rtabs](QKeyEvent* e) -> bool {
            if (e->modifiers() == Qt::ControlModifier) {
                if (e->key() == Qt::Key_N) {
                    return true; // DEPRECATED
                    addLeftTab_Scintilla(splitter, QString("(%1) ...").arg(index_new_tab));
                    index_new_tab++;
                    return true;
                }
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
    }
    // -- 
    window->setCentralWidget(splitter);
    window->resize(800, 600);
    window->show();
    auto result = app.exec();
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
            if (e->key() == Qt::Key_L) {
                QsciScintilla* newEditor = TabbedSplitView::dialogScintillaTabLoad(tabs);
                if (!newEditor) return true;
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
            if (e->key() == Qt::Key_N) {
                return true; // DEPRECATED
                if (!tabs) return true;
                QString tabs_name = tabs->objectName();
                QSplitter* splitter = CodexIncantation::findClosestParent<QSplitter>(tabs);
                if (!splitter) return true;
                if (tabs_name == "leftTabs") {
                    addLeftTab_Scintilla(splitter, QString("(%1)").arg(index_new_tab));
                    index_new_tab++;
                } else if (tabs_name == "rightTabs") {
                    addRightTab_Scintilla(splitter, QString("(%1)").arg(index_new_tab));
                    index_new_tab++;
                }
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

// -- END 
