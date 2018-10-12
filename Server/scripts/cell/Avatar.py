# -*- coding: utf-8 -*-
import KBEngine
from KBEDebug import *
import GameUtils
from interfaces.EntityCommon import EntityCommon

TIMER_TYPE_ADD_TRAP = 1

class Avatar(KBEngine.Entity, EntityCommon):
	def __init__(self):
		KBEngine.Entity.__init__(self)
		EntityCommon.__init__(self)

		# 设置每秒允许的最快速度, 超速会被拉回去
		#self.topSpeed = self.moveSpeed + 0.5
		self.topSpeed = 0.0

		# 随机的初始化一个出生位置
#		self.position = GameUtils.randomPosition3D(self.modelRadius)

		# self.topSpeedY = 10.0
		self.getCurrRoom().onEnter(self)


	def isAvatar(self):
		"""
		virtual method.
		"""
		return True

	#--------------------------------------------------------------------------------------------
	#                              Callbacks
	#--------------------------------------------------------------------------------------------
	def onTimer(self, tid, userArg):
		"""
		KBEngine method.
		引擎回调timer触发
		"""
		#DEBUG_MSG("%s::onTimer: %i, tid:%i, arg:%i" % (self.className, self.id, tid, userArg))


	def onEnterTrap(self, entityEntering, range_xz, range_y, controllerID, userarg):
		"""
		KBEngine method.
		有entity进入trap
		"""
		# 只有玩家实体进入，陷阱才工作
		pass



	def onLeaveTrap(self, entityLeaving, range_xz, range_y, controllerID, userarg):
		"""
		KBEngine method.
		有entity离开trap
		"""
		pass

	def onGetWitness(self):
		"""
		KBEngine method.
		绑定了一个观察者(客户端)
		"""
		DEBUG_MSG("Avatar::onGetWitness: %i." % self.id)

	def onLoseWitness(self):
		"""
		KBEngine method.
		解绑定了一个观察者(客户端)
		"""
		DEBUG_MSG("Avatar::onLoseWitness: %i." % self.id)

	def onDestroy(self):
		"""
		KBEngine method.
		entity销毁
		"""
		DEBUG_MSG("Avatar::onDestroy: %i." % self.id)
		room = self.getCurrRoom()

		if room:
			room.onLeave(self.id)
			

	def relive(self, exposed, type):
		"""
		defined.
		复活
		"""
		if exposed != self.id:
			return

		DEBUG_MSG("Avatar::relive: %i, type=%i." % (self.id, type))

	def stateChange(self,state):
		"""
		defined
		"""

		if self.state == state :
			return

		self.state = state

	def getState(self):
		"""
		defined
		"""
		return self.state









