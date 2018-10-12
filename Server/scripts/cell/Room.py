# -*- coding: utf-8 -*-
import KBEngine
from KBEDebug import *
import GameConfigs
import random
import GameUtils
import copy

from ROOM_INFO import TRoomInfo
from ROOM_LIST import TRoomList



TIMER_TYPE_ROOM_TICK = 1
TIMER_TYPE_DESTORY = 2

class Room(KBEngine.Entity):
	"""
	游戏场景
	"""
	def __init__(self):
		KBEngine.Entity.__init__(self)

		# 把自己移动到一个不可能触碰陷阱的地方
		self.position = (0.0, 0.0, 0.0)

		self.avatars = {}

		# 告诉客户端加载地图
		KBEngine.addSpaceGeometryMapping(self.spaceID, None, "spaces/gameMap")

		DEBUG_MSG('created space[%d] entityID = %i, res = %s.' % (self.roomKeyC, self.id, "spaces/gameMap"))

		# 让baseapp和cellapp都能够方便的访问到这个房间的entityCall
		KBEngine.globalData["Room_%i" % self.spaceID] = self.base

	def isInRoom(self,entityID):
		'''
		玩家是否在房间内
		'''
		if entityID in self.avatars:
			return True
		else:
			return False
		

#		DEBUG_MSG("Room::__init__,currFrame:%s,len:%i" % (str(self.currFrame),len(self.currFrame)))
	#--------------------------------------------------------------------------------------------
	#                              Callbacks
	#--------------------------------------------------------------------------------------------
	def onTimer(self, id, userArg):
		"""
		KBEngine method.
		使用addTimer后， 当时间到达则该接口被调用
		@param id		: addTimer 的返回值ID
		@param userArg	: addTimer 最后一个参数所给入的数据
		"""
#		DEBUG_MSG("Room::onTimer %d " % userArg)

		if userArg == TIMER_TYPE_DESTORY:
			self.onDestroyTimer()

	def onDestroy(self):
		"""
		KBEngine method.
		"""
		DEBUG_MSG("Room::onDestroy: %i" % (self.id))
		del KBEngine.globalData["Room_%i" % self.spaceID]

	def onDestroyTimer(self):
		DEBUG_MSG("Room::onDestroyTimer: %i" % (self.id))
		# 请求销毁引擎中创建的真实空间，在空间销毁后，所有该空间上的实体都被销毁
		self.destroySpace()


	def onEnter(self, entityCall):
		"""
		defined method.
		进入场景
		"""
		DEBUG_MSG('Room-cell::onEnter space[%d] entityID = %i.' % (self.spaceID, entityCall.id))
		self.avatars[entityCall.id] = entityCall

	def onLeave(self, entityID):
		"""
		defined method.
		离开场景
		"""
		DEBUG_MSG('Room::onLeave space[%d] entityID = %i.' % (self.spaceID, entityID))
		del self.avatars[entityID]



	def broadMessage(self,eventCode, message):

		for e in self.avatars.values():
			if e is None or e.client is None:
				continue
			e.client.component1.onTrueSyncData(eventCode,message)





		




