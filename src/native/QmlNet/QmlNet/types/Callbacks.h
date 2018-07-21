#ifndef NET_TYPE_INFO_MANAGER_H
#define NET_TYPE_INFO_MANAGER_H

#include <QmlNet.h>
#include <QmlNet/types/NetTypeInfo.h>
#include <QmlNet/types/NetPropertyInfo.h>
#include <QmlNet/types/NetMethodInfo.h>
#include <QmlNet/types/NetReference.h>
#include <QmlNet/qml/NetVariant.h>
#include <QmlNet/qml/NetVariantList.h>
#include <QSharedPointer>
#include <QString>

bool isTypeValid(QString type);

void releaseGCHandle(NetGCHandle* handle);

void buildTypeInfo(QSharedPointer<NetTypeInfo> typeInfo);

QSharedPointer<NetReference> instantiateType(QSharedPointer<NetTypeInfo> type);

void readProperty(QSharedPointer<NetPropertyInfo> property, QSharedPointer<NetReference> target, QSharedPointer<NetVariant> result);

void writeProperty(QSharedPointer<NetPropertyInfo> property, QSharedPointer<NetReference> target, QSharedPointer<NetVariant> value);

void invokeNetMethod(QSharedPointer<NetMethodInfo> method, QSharedPointer<NetReference> target, QSharedPointer<NetVariantList> parameters, QSharedPointer<NetVariant> result);

#endif // NET_TYPE_INFO_MANAGER_H
