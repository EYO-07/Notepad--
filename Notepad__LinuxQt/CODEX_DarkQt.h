#ifndef CODEX_DarkQt_H
#define CODEX_DarkQt_H
// -- 
#include <QtCore>
#include <QtGui>
#include <QtWidgets>
#include <QWidget>
#include <QString>
#include <QSplitter>
#include <QTextEdit>
#include <QObject>
#include <QKeyEvent>
#include <QTabWidget>

namespace CodexTransmutation {
    QString loadFile(QString pathFilename);
    void saveFile(QString pathFilename, QString);
    QString fileExists(QString path); // return "" or absolute path
    QString isDir(QString path); // return "" or absolute path 
    QString inputText(QWidget* parent, QString title, QString label, QString default_text);
    QString getShortFileName(QString absolutePath);
    bool isEnclosedBy(QString target, QString begin, QString end);
    QString getExtension(QString fileName);
}
namespace CodexIncantation {
    // theme 
    void applyDarkTheme(QWidget* root);
    void setWindowOpacity(QWidget* window, double level);
    // events
    void interceptKeyboardEvents(QObject* obj, std::function<bool(QKeyEvent*)> logic);
    // dialogs
    bool createEmptyFileDialog();
    // widgets
    void toggleOrientation(QSplitter *splitter);
    template<typename T> T* findClosestParent(QWidget* child);
    void moveSeparator(QSplitter* splitter, int value);
    // -- 
    namespace TabbedSplitView {
        QSplitter* tabbedSplitView();
        QSplitter* tabbedSplitView(QWidget*);
        int addLeftTab(QSplitter* view, QString name, QWidget& widget);
        int addRightTab(QSplitter* view, QString name, QWidget& widget);
        QTextEdit* addLeftTab_TextEdit(QSplitter* view, QString name);
        QTextEdit* addRightTab_TextEdit(QSplitter* view, QString name);
        QTabWidget* getTabsByName(QSplitter* view, QString tabs_name);
        // 0 continue 1 break ~ foreach 
        int foreachTab(QTabWidget* tabs, std::function<int(int,QWidget*,QTabWidget*)> fnc);
        int foreachTab(QTabWidget* tabs, std::function<int(int,QWidget*,QTabWidget*)> fnc, QObject*& find);
        // -- 
        void removeAndDestroyTab(QTabWidget* tabs, int index);
        void switchTabs(int index,QTabWidget* source, QTabWidget* dest);
        QSplitter* getSplitterFromWidget(QWidget* widget);
    }
    
}

// template definitions
template<typename T> T* CodexIncantation::findClosestParent(QWidget* child) {
    if (!child) return nullptr;
    QWidget* p = child->parentWidget();
    while (p) {
        T* result = qobject_cast<T*>(p);
        if (result) return result; 
        p = p->parentWidget();
    }
    return nullptr;
}

#endif // CODEX_DarkQt_H
