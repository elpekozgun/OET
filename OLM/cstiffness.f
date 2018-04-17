      subroutine cstiffness(n,nnmax,nelmax,nel,nf,elmat,
     +  xcoord,elnode,loads,u,v,steel,As,damage,steeln,
     +  eltype,strainm,beta,Es,ey,eult,sult,
     +  epscre,a1e,a2e,a3e,b1e,b2e,dsizee,Rxn)
      implicit none
      integer i,n,nelmax,ii,jj,g(4),m,nnmax,nel,j
      integer nf(nnmax,*),elnode(nelmax,*),steel(*),
     +  damage(*),i1,i2,steeln(*),eltype(*)
      double precision elmat(nelmax,*),xcoord(nnmax,*),loads(*)
      double precision cbeta,sbeta,KE(4,4),T(4,4),KET(4,4),ue(4),
     +  TTKET(4,4),TT(4,4),u(*),ey,eult,sult,Es,Rxn(*),
     +  nlength,strain,As(*),fg(4),fe(4),fe2(4),stress,EE,aa,bb,
     +  epsh,v(*),ve(4),strainm(*),cstrainth1,straink,
     +  a1e(*),a2e(*),a3e(*),b1e(*),b2e(*),epscre(*),dsizee(*),
     +  Fcr,F,EA,ecr,dsize,a11,a22,a33,
     +  a1,a2,a3,b1,b2,
     +  vxp1,vxp2,vyp1,vyp2,beta
     
      Rxn(1)=0.
      Rxn(2)=0.

      do i=1,n
        loads(i)=0.
      enddo
      
      do i=1,nel
      cstrainth1=epscre(eltype(i))
      
      i1=elnode(i,1)
      i2=elnode(i,2)

      m=0
      do ii=1,2
      do jj=1,2
          m=m+1
          g(m)=nf(elnode(i,ii),jj)
          if(g(m).ne.0)then
            ue(m)=u(g(m))
            ve(m)=v(g(m))
          else
            ue(m)=0.
            ve(m)=0.
          endif  
      enddo
      enddo

      nlength=(xcoord(elnode(i,1),1)-xcoord(elnode(i,2),1))**2.
     +   +(xcoord(elnode(i,1),2)-xcoord(elnode(i,2),2))**2.
           
      nlength=dsqrt(nlength)     
      strain=(nlength-elmat(i,3))/elmat(i,3)
      straink=strain
                  
      if(strain.ge.strainm(i))strainm(i)=strain
      if(strain.ge.0.0d00)then
         strain=strainm(i)
      endif
           
      if(damage(i).eq.0)then
        if(steel(i).eq.0.and.strain.ge.cstrainth1)then
            damage(i)=1            
         endif
      else
        damage(i)=1
      endif
      
      if(steel(i).eq.0.and.damage(i).eq.0)then
          aa=elmat(i,1)
      endif
      
      if(steel(i).eq.0.and.damage(i).eq.1)then

        if(steeln(i1).eq.1.or.steeln(i2).eq.1)then
c
c     !!!! elastoplastic
c
            if(strain.lt.0.)then
                aa=1.
            else
                ecr=cstrainth1
                Fcr=elmat(i,1)*ecr                   
                EA=elmat(i,1)
                aa=Fcr/(strain*EA)                
            endif
        else
            if(strain.lt.0.)then
                aa=1.
            else
            a11=a1e(eltype(i))
            a22=a2e(eltype(i))
            a33=a3e(eltype(i))            
            b1=b1e(eltype(i))
            b2=b2e(eltype(i))                        
            dsize=dsizee(eltype(i))
            
            ecr=cstrainth1
            Fcr=elmat(i,1)*ecr        
            EA=elmat(i,1)               

            a1=b1*Fcr*(elmat(i,3)-dsize)/elmat(i,1)+a11*ecr*dsize
            a2=b2*Fcr*(elmat(i,3)-dsize)/elmat(i,1)+a22*ecr*dsize
            a3=a33*ecr*dsize
            a1=a1/(ecr*elmat(i,3))
            a2=a2/(ecr*elmat(i,3))
            a3=a3/(ecr*elmat(i,3))      

            if(a1.lt.1.001)a1=1.001
            if(a2.lt.1.002)a2=1.002
            if(a3.lt.1.003)a3=1.003   
             
            if(strain.ge.ecr.and.strain.le.(a1*ecr))then
               F=(b1*Fcr-Fcr)/(a1*ecr-ecr)*(strain-ecr)+Fcr
               aa=F/(strain*EA)
            endif
            if(strain.ge.(a1*ecr).and.strain.le.(a2*ecr))then
              F=(b2*Fcr-b1*Fcr)/(a2*ecr-a1*ecr)*(strain-a1*ecr)+b1*Fcr
              aa=F/(strain*EA)
            endif
            if(strain.ge.(a2*ecr).and.strain.le.(a3*ecr))then
              F=(0.0-b2*Fcr)/(a3*ecr-a2*ecr)*(strain-a2*ecr)+b2*Fcr
              aa=F/(strain*EA)
            endif
            if(strain.ge.(a3*ecr))then
              aa=0.0
              F=0.
            endif        

           endif    
        endif

        aa=elmat(i,1)*aa
      endif
      
      if(steel(i).eq.1)then
          if(strain.le.ey)then
             aa=elmat(i,1)
          else
              if(strain.le.eult)then
                Fcr=elmat(i,1)*ey
                F=(sult*As(i)-Fcr)/(eult-ey)*(strain-ey)+Fcr
                aa=F/strain
              else
                 aa=0.0
              endif
          endif
c          call formd6(aa,elmat(i,2),elmat(i,3),KE)
      endif
    
      cbeta=xcoord(elnode(i,2),1)-xcoord(elnode(i,1),1)
      sbeta=xcoord(elnode(i,2),2)-xcoord(elnode(i,1),2)
      cbeta=cbeta/elmat(i,3)
      sbeta=sbeta/elmat(i,3)

      call formTT6(cbeta,sbeta,TT)
c      call matran(TT,4,T,4,4,4)
     
         fe(1)=-aa*straink
         fe(2)=0.
         fe(3)=aa*straink
         fe(4)=0.
         
         do ii=1,4
            fg(ii)=0.
            do jj=1,4
               fg(ii)=fg(ii)+TT(ii,jj)*fe(jj)          
            enddo
         enddo
         
         do ii=1,4
            if(g(ii).ne.0)then
                loads(g(ii))=loads(g(ii))-fg(ii)
            else
                if(ii.eq.1.or.ii.eq.3)Rxn(1)=Rxn(1)-fg(ii)
                if(ii.eq.2.or.ii.eq.4)Rxn(2)=Rxn(2)-fg(ii)
            endif
         enddo
         
         vxp1=cbeta*ve(1)+sbeta*ve(2)
         vyp1=-sbeta*ve(1)+cbeta*ve(2)
         vxp2=cbeta*ve(3)+sbeta*ve(4)
         vyp2=-sbeta*ve(3)+cbeta*ve(4)        
          
         fe(1)=-elmat(i,1)/elmat(i,3)*(vxp2-vxp1)*beta
         fe(2)=0.
         fe(3)=-fe(1)
         fe(4)=0.         
         
         do ii=1,4
            fg(ii)=0.
            do jj=1,4
               fg(ii)=fg(ii)+TT(ii,jj)*fe(jj)          
            enddo
         enddo
         
         do ii=1,4
            if(g(ii).ne.0)then
                loads(g(ii))=loads(g(ii))-fg(ii)
            endif
         enddo
      enddo

      return
      end

      double precision function damping(alpha,damage)
      implicit none
      double precision damage,alpha
      
c      damping=alpha/(0.1+damage**5.)
      damping=alpha
      
      return
      end