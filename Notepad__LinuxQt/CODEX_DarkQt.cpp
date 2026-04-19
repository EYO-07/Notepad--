// {TextMarker|cyan:>>>,<<<,TODO|red:ISSUE|yellow:INCOMPLETE,DEPRECATED,TESTING|silver:SCULPT}
// -- BEGIN
// CODEX_DarkQt.cpp
#include "CODEX_DarkQt.h"
#include <QApplication>
#include <QList>
#include <QFileInfo>
#include <QDir>
using namespace CodexIncantation;

// Transmutation
void CodexTransmutation::saveFile(QString fileName, QString content) { // Revision
    QFile file(fileName);
    if (!file.exists()) return;
    if (file.open(QIODevice::WriteOnly | QIODevice::Text)) {
        QTextStream out(&file);
        out << content;
        file.close();
    }
}
QString CodexTransmutation::loadFile(QString fileName) { // Revision 
    QFile file(fileName);
    // Check if it's a file and not a directory/socket
    if (!file.exists()) return "";
    // Safety Check: Don't load massive files
    // if (file.size() > 100 * 1024 * 1024) return ""; // 100MB limit
    if (!file.open(QIODevice::ReadOnly | QIODevice::Text)) return "";
    QTextStream in(&file);
    return in.readAll();
}
QString CodexTransmutation::fileExists(QString path) {
    /**
     * @brief Checks if a file exists and returns its absolute path.
     * @return Absolute path if it exists and is a file; otherwise empty string.
     */
    QFileInfo checkFile(path);
    if (checkFile.exists() && checkFile.isFile()) {
        return checkFile.absoluteFilePath();
    }
    return "";
}
QString CodexTransmutation::isDir(QString path) {
    /**
     * @brief Checks if a directory exists and returns its absolute path.
     * @return Absolute path if it exists and is a directory; otherwise empty string.
     */
    QFileInfo checkDir(path);
    if (checkDir.exists() && checkDir.isDir()) {
        return checkDir.absoluteFilePath();
    }
    return "";
}
QString CodexTransmutation::inputText(QWidget* parent, QString title, QString label, QString default_text) {
    bool ok;
    QString text = QInputDialog::getText(parent,title,label,QLineEdit::Normal,default_text,&ok);
    if (ok && !text.isEmpty()) {
        return text;
    } else {
        return "";
    }
}
QString CodexTransmutation::getShortFileName(QString absolutePath) {
    if (absolutePath.isEmpty()) return "untitled";    
    QFileInfo fileInfo(absolutePath);
    return fileInfo.fileName(); // Returns "main.cpp" from "/home/user/main.cpp"
}
bool CodexTransmutation::isEnclosedBy(QString target, QString begin, QString end) {
    return target.startsWith(begin) && target.endsWith(end);
}
QString CodexTransmutation::getExtension(QString fileName) {
    if (fileName.isEmpty()) return "";   
    QFileInfo fileInfo(fileName);
    return fileInfo.suffix().toLower();
}

// Incantation 
void CodexIncantation::applyDarkTheme(QWidget* root) {
    // 1. Force Fusion style to ensure cross-platform consistency
    qApp->setStyle(QStyleFactory::create("Fusion"));
    // 2. Prepare the root window for OS-level transparency
    root->setAttribute(Qt::WA_TranslucentBackground);
    root->setAutoFillBackground(false);
    // 3. Apply the Style Sheet
    // This targets the specific "viewport" and "pane" elements that ignore simple transparency
    QString default_background_color = "0,0,0"; // %1
    QString default_string_color = "white"; // %2
    QString background_transparency = "120"; // %3
    // -- 
    QString style = QString(
        // %1 default_background_color
        // %2 default_string_color
        // %3 background_transparency 
        /* Global Defaults */
        "QWidget { color: %2; background: rgba(%1,%3); font-family: system-ui; }"

        /* This is what you specifically asked for: The Selection colors */
        "QsciScintilla QListWidget::item:selected {"
        "    background-color: rgb(0,0,200);"
        "    color: %2;"
        "}"

        /* Top-level Splitter and Handles */
        "QSplitter { background: rgba(%1,%3); }"
        "QSplitter::handle { background: rgba(80, 80, 80, 100); }"
        "QSplitter::handle:horizontal { width: 4px; }"

        /* Tab Widget Styling */
        "QTabWidget::pane { border: 1px transparent; background: rgba(%1,100); }"
        "QTabBar::tab { background: rgba(%1,%3); padding: 5px 10px; }"
        "QTabBar::tab:selected { background: rgba(0,70,220,%3); border-bottom: 1px solid blue; color: white; }"
        "QTabBar::tab:hover { background: rgba(50,50,100,%3); }"

        /* The "Glass" Editors and Lists */
        "QTextEdit, QListWidget, QPlainTextEdit {"
        "   background-color: rgba(%1, %3);"
        "   selection-background-color: #00A3FF;"
        "   selection-color: black;"
        "}"

        /* CRITICAL: Target the internal viewports of scroll areas */
        "QAbstractScrollArea > QWidget > QWidget {"
        "   background: transparent;"
        "}"

        /* Scrollbars (Optional, but makes it look better) */
        "QScrollBar:vertical { background: transparent; width: 10px; }"
        "QScrollBar::handle:vertical { background: rgba(100, 100, 100, 100); min-height: 20px; border-radius: 5px; }"
        "QScrollBar:horizontal { background: transparent; height: 10px; }"
        "QScrollBar::handle:horizontal { background: rgba(100, 100, 100, 100); min-width: 20px; border-radius: 5px; }"
    )
        .arg(default_background_color)
        .arg(default_string_color)
        .arg(background_transparency)
    ;
    qApp->setStyleSheet(style);
}
void CodexIncantation::toggleOrientation(QSplitter *splitter) {
    if (splitter->orientation() == Qt::Horizontal) {
        splitter->setOrientation(Qt::Vertical);
    } else {
        splitter->setOrientation(Qt::Horizontal);
    }
}
bool CodexIncantation::createEmptyFileDialog() {
    QString fileName = QFileDialog::getSaveFileName(
        nullptr, 
        "Create Empty File", 
        "", 
        "All Files (*)"
    );
    if (fileName.isEmpty()) return false; 
    QFile file(fileName);
    if (file.open(QIODevice::WriteOnly)) {
        file.close();
        return true;
    } 
    return false;
}
void CodexIncantation::moveSeparator(QSplitter* splitter, int value) { // TESTING 
    if (!splitter) return;
    if (value == 0) return;
    // Get current sizes of all widgets in the splitter
    QList<int> sizes = splitter->sizes();
    if (sizes.size() < 2) return; // need at least two widgets
    // We’ll adjust the first two widgets for simplicity.
    int left = sizes[0];
    int right = sizes[1];
    if (value > 0) {
        // Move separator to the right (vertical splitter) or down (horizontal splitter)
        left += value;
        right -= value;
    } else {
        // Move separator to the left (vertical splitter) or up (horizontal splitter)
        left += value;   // value is negative
        right -= value;  // subtracting a negative increases right
    }
    // Prevent collapsing any widget completely
    if (left < 0) left = 0;
    if (right < 0) right = 0;
    sizes[0] = left;
    sizes[1] = right;
    splitter->setSizes(sizes);
}

// Incantation || namespace TabbedSplitView 
QSplitter* TabbedSplitView::tabbedSplitView() {
    return TabbedSplitView::tabbedSplitView(nullptr);
}
QSplitter* TabbedSplitView::tabbedSplitView(QWidget* parent) {
    QSplitter* splitter = new QSplitter(Qt::Horizontal,parent);
    QTabWidget* left = new QTabWidget(splitter);
    QTabWidget* right = new QTabWidget(splitter);
    left->setObjectName("leftTabs");
    right->setObjectName("rightTabs");
    return splitter;
}
int TabbedSplitView::addLeftTab(QSplitter* view, QString name, QWidget& widget) {
    QTabWidget* tabs = view->findChild<QTabWidget*>("leftTabs");
    if (tabs) {
        return tabs->addTab(&widget, name);
    }
    return -1;
}
int TabbedSplitView::addRightTab(QSplitter* view, QString name, QWidget& widget) {
    QTabWidget* tabs = view->findChild<QTabWidget*>("rightTabs");
    if (tabs) {
        return tabs->addTab(&widget, name);
    }
    return -1;
}
QTextEdit* TabbedSplitView::addLeftTab_TextEdit(QSplitter* view, QString name) {
    QTextEdit* editor = new QTextEdit();
    QTabWidget* tabs = view->findChild<QTabWidget*>("leftTabs");
    if (tabs) {
        tabs->addTab(editor, name);
        return editor;
    }
    return nullptr; // Safety if splitter is incorrect
}
QTextEdit* TabbedSplitView::addRightTab_TextEdit(QSplitter* view, QString name) {
    QTextEdit* editor = new QTextEdit();
    QTabWidget* tabs = view->findChild<QTabWidget*>("rightTabs");
    
    if (tabs) {
        tabs->addTab(editor, name);
        return editor;
    }
    return nullptr;
}
QTabWidget* TabbedSplitView::getTabsByName(QSplitter* view, QString tabs_name) {
    if (!view) return nullptr;
    return view->findChild<QTabWidget*>(tabs_name); // Not found
}
int TabbedSplitView::foreachTab(QTabWidget* tabs, std::function<int(int,QWidget*,QTabWidget*)> fnc) {
    QObject* dummy = nullptr;
    return TabbedSplitView::foreachTab(tabs,fnc,dummy);
}
int TabbedSplitView::foreachTab(QTabWidget* tabs, std::function<int(int,QWidget*,QTabWidget*)> fnc, QObject*& find) {
    // 0 - continue
    // 1 - break
    // 2 - object find 
    int result = 0;
    int tab_count = tabs->count();
    if (tab_count<=0) return 0;
    for (int idx=0;idx<tab_count;idx++) {
        QWidget* widget = tabs->widget(idx);
        result = fnc(idx,widget,tabs);
        if (result == 0) continue;
        if (result == 1) break;
        if (result == 2) {
            find = widget;
            break;
        }
    }        
    return result;
}
void TabbedSplitView::removeAndDestroyTab(QTabWidget* tabs, int index) {
    if (index < 0 || index >= tabs->count()) return;
    QWidget* page = tabs->widget(index);
    tabs->removeTab(index);
    if (page) page->deleteLater(); 
}
void TabbedSplitView::switchTabs(int index, QTabWidget* source, QTabWidget* dest) {
    if (!source || !dest || index < 0 || index >= source->count()) return; 
    // Extract metadata and the widget pointer
    QString label = source->tabText(index);
    QIcon icon = source->tabIcon(index);
    QString toolTip = source->tabToolTip(index);
    QVariant data = source->tabBar()->tabData(index);
    QWidget* page = source->widget(index);
    if (!page) return;
    // Remove from source (this doesn't delete 'page')
    source->removeTab(index);
    // Add to destination
    int newIndex = dest->addTab(page, icon, label);
    dest->setTabToolTip(newIndex, toolTip);
    dest->tabBar()->setTabData(newIndex, data);
    // Set the moved tab as the active one
    dest->setCurrentIndex(newIndex);
    dest->widget(newIndex)->setFocus();
}
QSplitter* TabbedSplitView::getSplitterFromWidget(QWidget* widget) {
    return findClosestParent<QSplitter>(widget);
}

// Incantation || Events
void CodexIncantation::interceptKeyboardEvents(QObject* obj, std::function<bool(QKeyEvent*)> logic) {
    if (!obj) return;
    // We create a "Lambda Object" on the fly. 
    // In Qt, we can use a generic QObject and a dynamic property or a tiny internal class.
    struct Interceptor : public QObject {
        std::function<bool(QKeyEvent*)> fn;
        Interceptor(QObject* p, std::function<bool(QKeyEvent*)> f) : QObject(p), fn(f) {}
        
        bool eventFilter(QObject*, QEvent* e) override {
            if (e->type() == QEvent::KeyPress) {
                return fn(static_cast<QKeyEvent*>(e));
            }
            return false;
        }
    };
    // Attach it. It will be deleted automatically when 'view' is deleted.
    obj->installEventFilter(new Interceptor(obj, logic));
}

// -- END 













