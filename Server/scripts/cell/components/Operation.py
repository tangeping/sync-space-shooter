# -*- coding: utf-8 -*-
import KBEngine
from KBEDebug import *
import GameConfigs

class Operation(KBEngine.EntityComponent):
	def __init__(self):
		KBEngine.EntityComponent.__init__(self)

	def onAttached(self, owner):
		"""
		"""
		INFO_MSG("Operation::onAttached(): owner=%i" % (owner.id))

	def onDetached(self, owner):
		"""
		"""
		INFO_MSG("Operation::onDetached(): owner=%i" % (owner.id))


	def reqTrueSyncData(self,exposed,eventCode,message):
		'''
		同步消息
		'''
		if exposed  != self.owner.id:
			return

		self.owner.getCurrRoom().broadMessage(eventCode,message)

	def reqLeaveRoom(self,exposed):
		"""
		exposed.
		客户端请求退出房间
		"""
		if exposed != self.owner.id:
			return

		result = GameConfigs.RESULT_OK

		if self.owner.getCurrRoom() is None :
			result = GameConfigs.EROOR_ROOM_DESTORYED

		KBEngine.globalData["Halls"].leaveRoom(self.ownerID, self.owner.roomKeyC)
		
		self.allClients.onLeaveRoomResult(result)

		DEBUG_MSG("Operation[%i].reqLeaveRoom:%d " % (self.ownerID,self.owner.roomKeyC))

	def reqGameBegin(self,exposed):
		"""
		exposed.
		客户端请求开始游戏
		"""
		if exposed != self.owner.id:
			return
			
		state = GameConfigs.ROOM_STATE_FREE
		currRoomEntity = self.owner.getCurrRoom()

		if currRoomEntity is None:
			return

		if currRoomEntity.state != GameConfigs.ROOM_STATE_FREE:
			state = currRoomEntity.state

		currRoomEntity.onStateChange(state)
		self.allClients.onGameBeginResult(state)

		DEBUG_MSG("Operation[%i].reqGameBegin:%d " % (self.ownerID,self.owner.roomKeyC))
