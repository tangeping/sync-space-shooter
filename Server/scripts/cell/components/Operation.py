# -*- coding: utf-8 -*-
import KBEngine
from KBEDebug import *

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

		
