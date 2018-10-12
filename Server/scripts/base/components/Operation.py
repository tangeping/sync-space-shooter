# -*- coding: utf-8 -*-
import KBEngine
import GameConfigs
from KBEDebug import *
from ROOM_INFO import TRoomInfo
from ROOM_LIST import TRoomList



class Operation(KBEngine.EntityComponent):
	"""docstring for Operation"""
	def __init__(self):
		KBEngine.EntityComponent.__init__(self)
		self.currRoomEntity = None

	def onAttached(self, owner):
		"""
		"""
		INFO_MSG("Operation::onAttached(): owner=%i" % (owner.id))

	def onDetached(self, owner):
		"""
		"""
		INFO_MSG("Operation::onDetached(): owner=%i" % (owner.id))

	def onClientEnabled(self):
		"""
		KBEngine method.
		该entity被正式激活为可使用， 此时entity已经建立了client对应实体， 可以在此创建它的
		cell部分。
		"""
		INFO_MSG("Operation[%i]::onClientEnabled:entities enable." % (self.ownerID))


	def onClientDeath(self):
		"""
		KBEngine method.
		客户端对应实体已经销毁
		"""
		DEBUG_MSG("Operation[%i].onClientDeath:" % self.ownerID)

	def onTimer(self, tid, userArg):
		"""
		KBEngine method.
		引擎回调timer触发
		"""
		DEBUG_MSG("%s::onTimer: %i, tid:%i, arg:%i" % (self.name, self.ownerID, tid, userArg))


	def onRoomInfo(self,roomData):
		'''
		'''
		if type(roomData) is not dict:
			return None
		keys = ['roomKey','PlayerCount','roomEntityCall','creater']

		for k in keys:
			if k not in roomData:
				return None

		roomEntity = roomData["roomEntityCall"]
		roomState =  roomEntity.roomState if (roomEntity is not None) else  GameConfigs.ROOM_STATE_UNKNOW

		Roominfo = TRoomInfo().createFromDict({
		"room_key":roomData['roomKey'],
		"player_count":roomData['PlayerCount'],
		"room_state":roomState,
		"room_creater":roomData['creater']})
		return Roominfo


	def reqRoomList(self):
		"""
		exposed.
		客户端请求房间列表
		"""

		rooms =  KBEngine.globalData["Halls"].reqRoomList(self.ownerID)
		#DEBUG_MSG("Operation::reqRoomList,rooms:%s" % str(rooms))

		roomList = TRoomList()
		for roomKey,roomData in rooms.items():
			roomInfo = self.onRoomInfo(roomData)
			if roomInfo is None:
				continue
			roomList[roomKey] = roomInfo

		self.client.onReqRoomList(roomList)
					
		DEBUG_MSG("Operation[%i].reqRoomList: %s" % (self.ownerID,str(roomList)))

	def reqCreateRoom(self):
		roomData =  KBEngine.globalData["Halls"].createRoom(self.ownerID,0)
		roomInfo = self.onRoomInfo(roomData)
		if roomInfo is None:
			return
		self.client.onCreateRoomResult(0,roomInfo)

	def reqEnterRoom(self,roomKey):
		"""
		exposed.
		客户端请求进入房间
		"""
		rooms =  KBEngine.globalData["Halls"].reqRoomList(self.ownerID)
		roomData = rooms.get(roomKey,None)
		roomInfo = self.onRoomInfo(roomData)
		result = GameConfigs.RESULT_OK

		if roomData is None or roomInfo is None:
			result = GameConfigs.EROOR_ROOM_NOT_FUND
		elif roomData['roomEntityCall'] is None:
			result = GameConfigs.EROOR_ROOM_CREATIN
		elif self.owner.isInRoom():
			result = GameConfigs.EROOR_ROOM_ENTERED

		self.currRoomEntity = roomData['roomEntityCall']
			
		self.client.onEnterRoomResult(result,roomInfo)			
		KBEngine.globalData["Halls"].enterRoom(self.owner, (0.0,0.0,0.0), (0.0,0.0,0.0), roomKey)	
		DEBUG_MSG("Operation[%i].reqEnterRoom: " % (self.ownerID))

	def reqLeaveRoom(self):
		"""
		exposed.
		客户端请求退出房间
		"""
		result = GameConfigs.RESULT_OK

		if self.currRoomEntity is None:
			result = GameConfigs.EROOR_ROOM_DESTORYED

		KBEngine.globalData["Halls"].leaveRoom(self.ownerID, self.currRoomEntity.roomKey)
		
		if self.owner.isInRoom():
			result = GameConfigs.EROOR_ROOM_EXIT_FAIL
		else:
			self.currRoomEntity = None

		self.client.onLeaveRoomResult(result)

		DEBUG_MSG("Operation[%i].reqLeaveRoom: " % (self.ownerID))

	def reqGameBegin(self):
		"""
		exposed.
		客户端请求开始游戏
		"""
		state = GameConfigs.ROOM_STATE_FREE

		if self.currRoomEntity is None:
			return

		if self.currRoomEntity.roomState != GameConfigs.ROOM_STATE_FREE:
			state = self.currRoomEntity.roomState

		self.currRoomEntity.roomState = GameConfigs.ROOM_STATE_PLAYING
		self.client.onGameBeginResult(state)

		DEBUG_MSG("Operation[%i].reqGameBegin: " % (self.ownerID))